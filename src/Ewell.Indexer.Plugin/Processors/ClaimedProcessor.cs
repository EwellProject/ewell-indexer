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
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[Claimed] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }
        var claimedAmount = eventValue.Amount;
        await UpdateProjectAsync(context, crowdfundingProject, claimedAmount);
        await UpdateUserProjectInfoAsync(context, crowdfundingProject.Id, user, claimedAmount);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Claim, 
            0, claimedAmount);
        Logger.LogInformation("[Claimed] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task UpdateProjectAsync(LogEventContext context, 
        CrowdfundingProjectIndex crowdfundingProject,
        long claimAmount)
    {
        crowdfundingProject.CurrentCrowdFundingIssueAmount -= claimAmount;
        ObjectMapper.Map(context, crowdfundingProject);
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user, long claimAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[Claimed] user project info with id {id} does not exist.", userProjectId);
            return;
        }
        userProjectInfo.ActualClaimAmount += claimAmount;
        userProjectInfo.ToClaimAmount -= claimAmount;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
    }
}