using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using EwellIndexer.Processors.Dto;

namespace EwellIndexer.Processors;

public class InvestedProcessor : UserProjectProcessorBase<Invested>
{
    public override async Task ProcessAsync(Invested logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var user = logEvent.User.ToBase58();
        Logger.LogInformation("[Invested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[Invested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        var result = await IsNewParticipantAsync(context, crowdfundingProject, user,
            logEvent.Amount, logEvent.ToClaimAmount);
        var isNewParticipant = result.IsNewParticipant;
        var lastClaimAmount = result.LastClaimAmount;
        if (isNewParticipant)
        {
            crowdfundingProject.ParticipantCount += 1;
        }

        crowdfundingProject.CurrentCrowdFundingIssueAmount -= lastClaimAmount;
        crowdfundingProject.CurrentCrowdFundingIssueAmount += logEvent.ToClaimAmount;
        crowdfundingProject.CurrentRaisedAmount += logEvent.Amount;
        await SaveEntityAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Invest,
            logEvent.Amount, logEvent.ToClaimAmount);
        Logger.LogInformation("[Invested] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task<IsNewParticipantResult> IsNewParticipantAsync(LogEventContext context,
        CrowdfundingProjectIndex crowdfundingProject,
        string user, long investAmount, long toClaimAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, crowdfundingProject.Id, user);
        var userProjectInfo = await GetEntityAsync<UserProjectInfoIndex>(userProjectId);

        if (userProjectInfo == null)
        {
            userProjectInfo = new UserProjectInfoIndex()
            {
                Id = userProjectId,
                User = user,
                CrowdfundingProjectId = crowdfundingProject.Id,
                InvestAmount = investAmount,
                ToClaimAmount = toClaimAmount,
                CrowdfundingProject = crowdfundingProject,
                CreateTime = context.Block.BlockTime
            };
            await SaveEntityAsync(userProjectInfo);
            return new IsNewParticipantResult
            {
                IsNewParticipant = true, LastClaimAmount = 0
            };
        }

        var originInvestAmount = userProjectInfo.InvestAmount;
        userProjectInfo.InvestAmount = originInvestAmount + investAmount;
        var lastClaimAmount = userProjectInfo.ToClaimAmount;
        userProjectInfo.ToClaimAmount = toClaimAmount;
        userProjectInfo.CreateTime = context.Block.BlockTime;
        await SaveEntityAsync(userProjectInfo);
        return new IsNewParticipantResult
        {
            IsNewParticipant = originInvestAmount == 0, LastClaimAmount = lastClaimAmount
        };
    }
}