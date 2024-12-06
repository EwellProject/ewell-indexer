using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class ClaimedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var invested = Invested();
        await MockEventProcess(invested, InvestedProcessor);
        var claimed = Claimed();
        await MockEventProcess(claimed, ClaimedProcessor);
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.CurrentRaisedAmount.ShouldBe(invested.Amount);
        projectIndex.CurrentCrowdFundingIssueAmount.ShouldBe(invested.ToClaimAmount - claimed.Amount);
        
        var userRecordId = IdGenerateHelper.GetId(ChainId, projectId, BobAddress, 
            BehaviorType.Claim, transactionId);
        var userRecordIndex = await GetIndexById<UserRecordIndex>(userRecordId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(ChainId, claimed.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await GetIndexById<UserProjectInfoIndex>(userProjectId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.ActualClaimAmount.ShouldBe(claimed.Amount);
        userProjectInfoIndex.ToClaimAmount.ShouldBe(invested.ToClaimAmount - claimed.Amount);
    }
}