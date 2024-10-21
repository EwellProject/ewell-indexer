using System.Threading.Tasks;
using AeFinder.Sdk.Processor;
using AElf.CSharp.Core;
using EwellIndexer.Entities;

namespace EwellIndexer.Processors.Base;

public abstract class UserProjectProcessorBase<TEvent> : ProjectProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    protected async Task AddUserRecordAsync(LogEventContext context, CrowdfundingProjectIndex crowdfundingProject,
        string user, BehaviorType behaviorType,
        long toRaiseTokenAmount, long crowdFundingIssueAmount)
    {
        var userRecordId = IdGenerateHelper.GetId(context.ChainId, crowdfundingProject.Id, user,
            behaviorType, context.Transaction.TransactionId);
        var userRecordIndex = new UserRecordIndex
        {
            Id = userRecordId,
            User = user,
            CrowdfundingProjectId = crowdfundingProject.Id,
            BehaviorType = behaviorType,
            ToRaiseTokenAmount = toRaiseTokenAmount,
            CrowdFundingIssueAmount = crowdFundingIssueAmount,
            DateTime = context.Block.BlockTime,
            CrowdfundingProject = crowdfundingProject,
            ToRaiseToken = crowdfundingProject.ToRaiseToken,
            CrowdFundingIssueToken = crowdfundingProject.CrowdFundingIssueToken
        };
        await SaveEntityAsync(userRecordIndex);
    }
}