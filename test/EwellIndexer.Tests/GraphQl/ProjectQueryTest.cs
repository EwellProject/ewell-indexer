using EwellIndexer.GraphQL;
using EwellIndexer.GraphQL.Input;
using Shouldly;
using Xunit;

namespace EwellIndexer.GraphQl;

public class ProjectQueryTest : EwellIndexerTestBase
{
    [Fact]
    public async Task GetProjectListAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await WhitelistDisabledLogEventProcessor.ProcessAsync(GenerateLogEventContext(WhitelistDisabled()));

        var userRecords = await Query.GetProjectListAsync(CrowdfundingProjectIndexRepository, WhitelistIndexRepository, ObjectMapper, new GetProjectListInput());
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetProjectListAsync(CrowdfundingProjectIndexRepository, WhitelistIndexRepository, ObjectMapper, new GetProjectListInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(1);
        
        userRecords = await Query.GetProjectListAsync(CrowdfundingProjectIndexRepository, WhitelistIndexRepository, ObjectMapper, new GetProjectListInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(0);
        userRecords.Data.Count.ShouldBe(0);
        
        userRecords = await Query.GetProjectListAsync(CrowdfundingProjectIndexRepository, WhitelistIndexRepository, ObjectMapper, new GetProjectListInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 1
        });
        userRecords.ShouldNotBeNull();
        userRecords.TotalCount.ShouldBe(1);
        userRecords.Data.Count.ShouldBe(0);
    }
}