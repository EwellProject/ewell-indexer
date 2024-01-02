using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class UnInvestedProcessor : AElfLogEventProcessorBase<UnInvested, LogEventInfo>
{
    private readonly ILogger<AElfLogEventProcessorBase<UnInvested, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> _crowdfundingProjectRepository;
    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;
    
    public UnInvestedProcessor(ILogger<AElfLogEventProcessorBase<UnInvested, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        ContractInfoOptions contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) : base(logger)
    {
        _logger = logger;
        _objectMapper = objectMapper;
        _contractInfoOptions = contractInfoOptions;
        _crowdfundingProjectRepository = crowdfundingProjectRepository;
        _userProjectInfoRepository = userProjectInfoRepository;
        _userRecordRepository = userRecordRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected override async Task HandleEventAsync(UnInvested eventValue, LogEventContext context)
    { 
        var projectHash = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        var projectId = IdGenerateHelper.GetProjectId(context.ChainId, projectHash);
        _logger.LogInformation("[UnInvested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject =
            await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            _logger.LogInformation("[UnInvested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        if (userProjectInfo == null)
        {
            _logger.LogInformation("[UnInvested] user project info with id {id} does not exist.", userProjectId);
            return;
        }
        var unInvestAmount = eventValue.UnInvestAmount;
        var userInvestedAmount = userProjectInfo.InvestAmount;
        var totalToClaimAmount = userProjectInfo.ToClaimAmount;
        var projectReceivableLiquidatedDamage = userInvestedAmount - unInvestAmount;
        userProjectInfo.InvestAmount = 0;
        userProjectInfo.ToClaimAmount = 0;
        _objectMapper.Map(context, userProjectInfo);
        await _userProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
        
        crowdfundingProject.CurrentRaisedAmount -= userInvestedAmount;
        crowdfundingProject.ReceivableLiquidatedDamageAmount += projectReceivableLiquidatedDamage;
        crowdfundingProject.CurrentCrowdFundingIssueAmount -= totalToClaimAmount;
        if (crowdfundingProject.ParticipantCount > 0)
        {
            crowdfundingProject.ParticipantCount -= 1;
        }
        _objectMapper.Map(context, crowdfundingProject);
        await _crowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, unInvestAmount, totalToClaimAmount);
        _logger.LogInformation("[UnInvested] end projectId:{projectId} user:{user} ", projectId, user);
    }
    
    private async Task AddUserRecordAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject, string user,
        long investAmount, long projectTokenAmount)
    {
        var userRecordId = IdGenerateHelper.GetId(context.ChainId, crowdfundingProject.Id, user, 
            BehaviorType.UnInvest, context.TransactionId);
        var userRecordIndex = new UserRecordIndex()
        {
            ChainId = context.ChainId,
            User = user,
            CrowdfundingProjectId = crowdfundingProject.Id,
            BehaviorType = BehaviorType.UnInvest,
            ToRaiseTokenAmount = investAmount,
            CrowdFundingIssueAmount = projectTokenAmount,
            DateTime = context.BlockTime,
            CrowdfundingProject = crowdfundingProject
        };
        await _userRecordRepository.AddOrUpdateAsync(userRecordIndex);
    }
}