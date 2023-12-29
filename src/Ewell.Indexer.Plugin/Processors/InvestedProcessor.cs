using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class InvestedProcessor : AElfLogEventProcessorBase<Invested, LogEventInfo>
{
    private readonly ILogger<AElfLogEventProcessorBase<Invested, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> _crowdfundingProjectRepository;
    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;
    
    public InvestedProcessor(ILogger<AElfLogEventProcessorBase<Invested, LogEventInfo>> logger,
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

    protected override async Task HandleEventAsync(Invested eventValue, LogEventContext context)
    {
        var projectHash = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        _logger.LogInformation("[Invested] start chainId:{chainId} user:{user} projectId:{projectHash}",
            context.ChainId,
            user, projectHash);
        var crowdfundingProject =
            await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectHash, context.ChainId);
        if (crowdfundingProject == null)
        {
            _logger.LogInformation("[Invested] crowd funding  project with id {id} does not exist.", projectHash);
            return;
        }

        var (isNewParticipant, lastClaimAmount) = await IsNewParicipantAsync(context, crowdfundingProject, user,
            eventValue.Amount, eventValue.ToClaimAmount, null, null);
        if (isNewParticipant)
        {
            crowdfundingProject.ParticipantCount += 1;
        }

        crowdfundingProject.CurrentCrowdFundingIssueAmount -= lastClaimAmount;
        crowdfundingProject.CurrentCrowdFundingIssueAmount += eventValue.ToClaimAmount;
        crowdfundingProject.CurrentRaisedAmount += eventValue.Amount;
        _objectMapper.Map(context, crowdfundingProject);
        await _crowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, eventValue.Amount,
            eventValue.ToClaimAmount, null, null);
        _logger.LogInformation("[Invested] end chainId:{chainId} user:{user} projectId:{projectHash} ", context.ChainId,
            user, projectHash);
    }

    private async Task<(bool, long)> IsNewParicipantAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        string user, long investAmount, long toClaimAmount, TokenBasicInfo toRaiseToken,
        TokenBasicInfo crowdFundingIssueToken)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, crowdfundingProject.Id, user);
        var userProjectInfo = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);

        if (userProjectInfo == null)
        {
            userProjectInfo = new UserProjectInfoIndex()
            {
                Id = userProjectId,
                ChainId = context.ChainId,
                User = user,
                CrowdfundingProjectId = crowdfundingProject.Id,
                InvestAmount = investAmount,
                ToClaimAmount = toClaimAmount,
                CrowdfundingProject = crowdfundingProject,
                ToRaiseToken = toRaiseToken,
                CrowdFundingIssueToken = crowdFundingIssueToken
            };
            _objectMapper.Map(context, userProjectInfo);
            await _userProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
            return (true, 0);
        }
        var originInvestAmount = userProjectInfo.InvestAmount;
        userProjectInfo.InvestAmount = originInvestAmount + investAmount;
        var lastClaimAmount = userProjectInfo.ToClaimAmount;
        userProjectInfo.ToClaimAmount = toClaimAmount;
        _objectMapper.Map(context, userProjectInfo);
        await _userProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
        return (originInvestAmount == 0, lastClaimAmount);
    }

    private async Task AddUserRecordAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        string user,
        long crowdFundingTokenAmount,
        long toClaimAmount, TokenBasicInfo crowdfundingToken, TokenBasicInfo publicOfferingToken)
    {
        var userRecordId = IdGenerateHelper.GetId(context.ChainId, user,
            crowdfundingProject.Id, "Invested", context.TransactionId);
        var userRecordIndex = new UserRecordIndex()
        {
            Id = userRecordId,
            ChainId = context.ChainId,
            User = user,
            CrowdfundingProjectId = crowdfundingProject.Id,
            BehaviorType = BehaviorType.Invest,
            ToRaiseTokenAmount = crowdFundingTokenAmount,
            CrowdFundingIssueAmount = toClaimAmount,
            DateTime = context.BlockTime,
            CrowdfundingProject = crowdfundingProject,
            ToRaiseToken = crowdfundingToken,
            CrowdFundingIssueToken = publicOfferingToken
        };
        _objectMapper.Map(context, userRecordIndex);
        await _userRecordRepository.AddOrUpdateAsync(userRecordIndex);
    }
}