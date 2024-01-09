using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class WhitelistQueryTest : QueryTestBase
{
    [Fact]
    public async Task GetWhitelistListAsync_Test()
    {
        await MockWhitelistDisable();
        
        var whitelists = await Query.GetWhitelistListAsync(_whitelistRepository, _objectMapper, new GetInputBase());
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(0);
        
        whitelists = await Query.GetWhitelistListAsync(_whitelistRepository, _objectMapper, new GetInputBase
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(1);
        
        whitelists = await Query.GetWhitelistListAsync(_whitelistRepository, _objectMapper, new GetInputBase
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(0);
        whitelists.Data.Count.ShouldBe(0);
        
        whitelists = await Query.GetWhitelistListAsync(_whitelistRepository, _objectMapper, new GetInputBase
        {
            ChainId = Chain_AELF,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 1
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(0);
    }
}