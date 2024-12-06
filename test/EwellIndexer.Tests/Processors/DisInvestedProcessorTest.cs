using AElf;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class DisInvestedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var invested = Invested();
        await MockEventProcess(invested, InvestedProcessor);
        var disInvested = DisInvested();
        await MockEventProcess(disInvested, DisInvestedProcessor);
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.CurrentRaisedAmount.ShouldBe(0);
        projectIndex.ReceivableLiquidatedDamageAmount.ShouldBe(invested.Amount - disInvested.DisinvestedAmount);
        
        var userRecordId = IdGenerateHelper.GetId(ChainId, projectId, BobAddress, 
            BehaviorType.Disinvest, transactionId);
        var userRecordIndex = await GetIndexById<UserRecordIndex>(userRecordId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(ChainId, disInvested.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await GetIndexById<UserProjectInfoIndex>(userProjectId);
        userProjectInfoIndex.ShouldNotBeNull();
        //check Remain InvestAmount
        userProjectInfoIndex.InvestAmount.ShouldBe(0);
        userProjectInfoIndex.ToClaimAmount.ShouldBe(0);
    }
}