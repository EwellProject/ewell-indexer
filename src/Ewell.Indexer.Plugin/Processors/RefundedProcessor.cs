using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class RefundedProcessor : AElfLogEventProcessorBase<ReFunded, LogEventInfo>
{
    private readonly ILogger<AElfLogEventProcessorBase<ReFunded, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;

    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;

    public RefundedProcessor(ILogger<AElfLogEventProcessorBase<ReFunded, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,        
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) : base(logger)
    {
        _logger = logger;
        _objectMapper = objectMapper;
        _contractInfoOptions = contractInfoOptions.Value;
        _crowdfundingProjectRepository = crowdfundingProjectRepository;
        _userProjectInfoRepository = userProjectInfoRepository;
        _userRecordRepository = userRecordRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected override async Task HandleEventAsync(ReFunded eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        _logger.LogInformation("[ReFunded] start projectId:{projectId} user:{user} ", projectId, user);
        var refundAmount = eventValue.Amount;
        var crowdfundingProject = await UpdateProjectAsync(context, projectId, refundAmount);
        await UpdateUserProjectInfoAsync(context, projectId, user, refundAmount);
        await AddUserRecordAsync(context, crowdfundingProject, user, refundAmount);
        _logger.LogInformation("[ReFunded] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task<CrowdfundingProjectIndex> UpdateProjectAsync(LogEventContext context, string projectId,
        long refundAmount)
    {
        var crowdfundingProject =
            await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        // crowdfundingProject.CurrentRaisedAmount -= refundAmount;
        // return await _crowdfundingProjectsRepository.UpdateAsync(crowdfundingProject);
        return crowdfundingProject;
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user,
        long refundAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        userProjectInfo.InvestAmount -= refundAmount;
        userProjectInfo.ToClaimAmount = 0;
        _objectMapper.Map(context, userProjectInfo);
        await _userProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
    }

    private async Task AddUserRecordAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        string user, long refundAmount)
    {
        var userRecordId = IdGenerateHelper.GetId(context.ChainId, crowdfundingProject.Id, user,
            BehaviorType.Refund, context.TransactionId);
        var userRecordIndex = new UserRecordIndex()
        {
            Id = userRecordId,
            ChainId = context.ChainId,
            User = user,
            CrowdfundingProjectId = crowdfundingProject.Id,
            BehaviorType = BehaviorType.Refund,
            ToRaiseTokenAmount = refundAmount,
            CrowdFundingIssueAmount = 0,
            DateTime = context.BlockTime,
            CrowdfundingProject = crowdfundingProject
        };
        _objectMapper.Map(context, userRecordIndex);
        await _userRecordRepository.AddOrUpdateAsync(userRecordIndex);
    }
}