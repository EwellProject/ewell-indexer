using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class RefundedProcessor : UserProjectProcessorBase<ReFunded>
{
    public override async Task ProcessAsync(ReFunded logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var user = logEvent.User.ToBase58();
        Logger.LogInformation("[ReFunded] start projectId:{projectId} user:{user} ", projectId, user);
        var refundAmount = logEvent.Amount;
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[ReFunded] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        await UpdateProjectAsync(context, crowdfundingProject, refundAmount);
        await UpdateUserProjectInfoAsync(context, projectId, user, refundAmount);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Refund,
            refundAmount, 0);
        Logger.LogInformation("[ReFunded] end projectId:{projectId} user:{user} ", projectId, user);
    }
    
    private async Task UpdateProjectAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        long refundAmount)
    {
        crowdfundingProject.CurrentRaisedAmount -= refundAmount;
        if (crowdfundingProject.ParticipantCount > 0)
        {
            crowdfundingProject.ParticipantCount -= 1;
        }
        await SaveEntityAsync(crowdfundingProject);
    }
    
    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user,
        long refundAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo = await GetEntityAsync<UserProjectInfoIndex>(userProjectId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[ReFunded] user project info with id {id} does not exist.", userProjectId);
            return;
        }
        userProjectInfo.InvestAmount -= refundAmount;
        userProjectInfo.ToClaimAmount = 0;
        await SaveEntityAsync(userProjectInfo);
    }
}