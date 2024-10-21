using EwellIndexer.GraphQL;
using EwellIndexer.GraphQL.Input;
using Shouldly;
using Xunit;

namespace EwellIndexer.GraphQl;

public class UserRecordQueryTest : EwellIndexerTestBase
{
    [Fact]
    public async Task GetUserRecordListAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await MockEventProcess(Invested(), InvestedProcessor);
        await MockEventProcess(DisInvested(), DisInvestedProcessor);
        
        var userRecords = await Query.GetUserRecordListAsync(UserRecordIndexRepository, ObjectMapper, new GetUserRecordInput());
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetUserRecordListAsync(UserRecordIndexRepository, ObjectMapper, new GetUserRecordInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(2);
        
        userRecords = await Query.GetUserRecordListAsync(UserRecordIndexRepository, ObjectMapper, new GetUserRecordInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(0);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetUserRecordListAsync(UserRecordIndexRepository, ObjectMapper, new GetUserRecordInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 2
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(0);
    }
}