using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ClaimedProcessor : UserProjectProcessorBase<Claimed>
{
    public override async Task ProcessAsync(Claimed logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var user = logEvent.User.ToBase58();
        Logger.LogInformation("[Claimed] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[Claimed] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }
        var claimedAmount = logEvent.Amount;
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
        await SaveEntityAsync(crowdfundingProject);
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user, long claimAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo = await GetEntityAsync<UserProjectInfoIndex>(userProjectId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[Claimed] user project info with id {id} does not exist.", userProjectId);
            return;
        }
        userProjectInfo.ActualClaimAmount += claimAmount;
        userProjectInfo.ToClaimAmount -= claimAmount;
        await SaveEntityAsync(userProjectInfo);
    }
}