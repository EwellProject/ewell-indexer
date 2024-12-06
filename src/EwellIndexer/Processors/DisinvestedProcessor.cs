using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class DisInvestedProcessor : UserProjectProcessorBase<DisInvested>
{
    public override async Task ProcessAsync(DisInvested logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var user = logEvent.User.ToBase58();
        Logger.LogInformation("[DisInvested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[DisInvested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo = await GetEntityAsync<UserProjectInfoIndex>(userProjectId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[DisInvested] user project info with id {id} does not exist.", userProjectId);
            return;
        }

        var disinvestedAmount = logEvent.DisinvestedAmount;
        var userInvestedAmount = userProjectInfo.InvestAmount;
        var totalToClaimAmount = userProjectInfo.ToClaimAmount;
        var projectReceivableLiquidatedDamage = userInvestedAmount - disinvestedAmount;
        //reset amount
        userProjectInfo.InvestAmount = 0;
        userProjectInfo.ToClaimAmount = 0;
        userProjectInfo.ActualClaimAmount = 0;
        userProjectInfo.LiquidatedDamageAmount = projectReceivableLiquidatedDamage;
        await SaveEntityAsync(userProjectInfo);

        crowdfundingProject.CurrentRaisedAmount -= userInvestedAmount;
        crowdfundingProject.ReceivableLiquidatedDamageAmount += projectReceivableLiquidatedDamage;
        crowdfundingProject.CurrentCrowdFundingIssueAmount -= totalToClaimAmount;
        if (crowdfundingProject.ParticipantCount > 0)
        {
            crowdfundingProject.ParticipantCount -= 1;
        }

        await SaveEntityAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Disinvest,
            disinvestedAmount, totalToClaimAmount);
        Logger.LogInformation("[DisInvested] end projectId:{projectId} user:{user} ", projectId, user);
    }
}