using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class UserRecordQueryTest : QueryTestBase
{
    [Fact]
    public async Task GetUserRecordListAsync_Test()
    {
        await MockProjectRegistered();
        await MockInvested();
        await MockDisinvest();
        
        var userRecords = await Query.GetUserRecordListAsync(_userRecordRepository, _objectMapper, new GetUserRecordInput());
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetUserRecordListAsync(_userRecordRepository, _objectMapper, new GetUserRecordInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(2);
        
        userRecords = await Query.GetUserRecordListAsync(_userRecordRepository, _objectMapper, new GetUserRecordInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(0);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetUserRecordListAsync(_userRecordRepository, _objectMapper, new GetUserRecordInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 2
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(2);
        userRecords.Data.Count.ShouldBe(0);
    }
}