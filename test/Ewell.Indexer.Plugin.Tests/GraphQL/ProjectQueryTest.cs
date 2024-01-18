using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class ProjectQueryTest : QueryTestBase
{
    [Fact]
    public async Task GetProjectListAsync_Test()
    {
        await MockProjectRegistered();
        await MockWhitelistDisable();

        var userRecords = await Query.GetProjectListAsync(_crowdfundingProjectRepository, _whitelistRepository, _objectMapper, new GetProjectListInput());
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetProjectListAsync(_crowdfundingProjectRepository, _whitelistRepository, _objectMapper, new GetProjectListInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(1);
        
        userRecords = await Query.GetProjectListAsync(_crowdfundingProjectRepository, _whitelistRepository, _objectMapper, new GetProjectListInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(0);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetProjectListAsync(_crowdfundingProjectRepository, _whitelistRepository, _objectMapper, new GetProjectListInput
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 1
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(0);
    }
}