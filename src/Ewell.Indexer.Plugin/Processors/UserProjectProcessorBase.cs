using AElf.Contracts.Ewell;
using AElf.CSharp.Core;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public abstract class UserProjectProcessorBase<TEvent> : AElfLogEventProcessorBase<TEvent, LogEventInfo>
    where TEvent : IEvent<TEvent>, new()
{
    protected readonly ILogger<AElfLogEventProcessorBase<TEvent, LogEventInfo>> Logger;
    protected readonly IObjectMapper ObjectMapper;
    protected readonly ContractInfoOptions ContractInfoOptions;

    protected readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        CrowdfundingProjectRepository;

    protected readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> UserProjectInfoRepository;
    protected readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> UserRecordRepository;

    protected UserProjectProcessorBase(ILogger<AElfLogEventProcessorBase<TEvent, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) : base(logger)
    {
        Logger = logger;
        ObjectMapper = objectMapper;
        ContractInfoOptions = contractInfoOptions.Value;
        CrowdfundingProjectRepository = crowdfundingProjectRepository;
        UserProjectInfoRepository = userProjectInfoRepository;
        UserRecordRepository = userRecordRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected async Task AddUserRecordAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        string user, BehaviorType behaviorType,
        long toRaiseTokenAmount, long crowdFundingIssueAmount)
    {
        var userRecordId = IdGenerateHelper.GetId(context.ChainId, crowdfundingProject.Id, user,
            behaviorType, context.TransactionId);
        var userRecordIndex = new UserRecordIndex()
        {
            Id = userRecordId,
            ChainId = context.ChainId,
            User = user,
            CrowdfundingProjectId = crowdfundingProject.Id,
            BehaviorType = BehaviorType.UnInvest,
            ToRaiseTokenAmount = toRaiseTokenAmount,
            CrowdFundingIssueAmount = crowdFundingIssueAmount,
            DateTime = context.BlockTime,
            CrowdfundingProject = crowdfundingProject
        };
        ObjectMapper.Map(context, userRecordIndex);
        await UserRecordRepository.AddOrUpdateAsync(userRecordIndex);
    }
}