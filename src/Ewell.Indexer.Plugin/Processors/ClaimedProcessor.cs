using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ClaimedProcessor : UserProjectProcessorBase<Claimed>
{
    public ClaimedProcessor(ILogger<AElfLogEventProcessorBase<Claimed, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    protected override async Task HandleEventAsync(Claimed eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        Logger.LogInformation("[Claimed] start projectId:{projectId} user:{user} ", projectId, user);
        var claimedAmount = eventValue.Amount;
        var crowdfundingProject = await UpdateProjectAsync(context, projectId, claimedAmount);
        await UpdateUserProjectInfoAsync(context, crowdfundingProject.Id, user, claimedAmount);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Claim, 
            0, claimedAmount);
        Logger.LogInformation("[Claimed] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task<CrowdfundingProjectIndex> UpdateProjectAsync(LogEventContext context, string projectId,
        long claimAmount)
    {
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        crowdfundingProject.CurrentCrowdFundingIssueAmount -= claimAmount;
        ObjectMapper.Map(context, crowdfundingProject);
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        return crowdfundingProject;
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user, long claimAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        userProjectInfo.ActualClaimAmount += claimAmount;
        userProjectInfo.ToClaimAmount -= claimAmount;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
    }
}