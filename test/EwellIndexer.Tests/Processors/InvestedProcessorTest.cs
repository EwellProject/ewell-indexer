using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class InvestedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var invested = Invested();
        await MockEventProcess(invested, InvestedProcessor);

        var projectId = invested.ProjectId.ToHex();
        var userProjectId = IdGenerateHelper.GetUserProjectId(ChainId, projectId, BobAddress);
        var userProjectInfoIndex = await GetIndexById<UserProjectInfoIndex>(userProjectId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.InvestAmount.ShouldBe(invested.Amount);
        
        var userRecordId = IdGenerateHelper.GetId(ChainId, projectId, BobAddress, 
            BehaviorType.Invest, transactionId);
        var userRecordIndex = await GetIndexById<UserRecordIndex>(userRecordId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
    }
}