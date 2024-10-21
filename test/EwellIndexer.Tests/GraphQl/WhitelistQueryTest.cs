using EwellIndexer.GraphQL;
using EwellIndexer.GraphQL.Input;
using Shouldly;
using Xunit;

namespace EwellIndexer.GraphQl;

public class WhitelistQueryTest : EwellIndexerTestBase
{
    [Fact]
    public async Task GetWhitelistListAsync_Test()
    {
        await WhitelistDisabledLogEventProcessor.ProcessAsync(GenerateLogEventContext(WhitelistDisabled()));
        
        var whitelists = await Query.GetWhitelistListAsync(WhitelistIndexRepository, ObjectMapper, new GetWhitelistInput());
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(0);
        
        whitelists = await Query.GetWhitelistListAsync(WhitelistIndexRepository, ObjectMapper, new GetWhitelistInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            EndBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(1);
        
        whitelists = await Query.GetWhitelistListAsync(WhitelistIndexRepository, ObjectMapper, new GetWhitelistInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight + 1,
            MaxResultCount = 10
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(0);
        whitelists.Data.Count.ShouldBe(0);
        
        whitelists = await Query.GetWhitelistListAsync(WhitelistIndexRepository, ObjectMapper, new GetWhitelistInput
        {
            ChainId = ChainId,
            StartBlockHeight = blockHeight,
            MaxResultCount = 10,
            SkipCount = 1
        });
        whitelists.ShouldNotBeNull();
        whitelists.TotalCount.ShouldBe(1);
        whitelists.Data.Count.ShouldBe(0);
    }
}