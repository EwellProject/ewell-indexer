using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class RefundedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var invested = Invested();
        await MockEventProcess(invested, InvestedProcessor);
        var reFunded = ReFunded();
        await MockEventProcess(reFunded, RefundedProcessor);

        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.ParticipantCount.ShouldBe(0);
        projectIndex.CurrentRaisedAmount.ShouldBe(invested.Amount - reFunded.Amount);
        
        var userRecordId = IdGenerateHelper.GetId(ChainId, projectId, BobAddress,
            BehaviorType.Refund, transactionId);
        var userRecordIndex = await GetIndexById<UserRecordIndex>(userRecordId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
        userRecordIndex.ToRaiseTokenAmount.ShouldBe(reFunded.Amount);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(ChainId, reFunded.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await GetIndexById<UserProjectInfoIndex>(userProjectId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.InvestAmount.ShouldBe(invested.Amount - reFunded.Amount);
        userProjectInfoIndex.ToClaimAmount.ShouldBe(0);
    }
}
