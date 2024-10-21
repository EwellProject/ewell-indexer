using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class TokenCreatedLogEventProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var symbol = "READ-1";
        await TokenCreatedLogEventProcessor.ProcessAsync(GenerateLogEventContext(TokenCreated(symbol)));
        var tokenId = IdGenerateHelper.GetTokenInfoId(ChainId, symbol);
        var tokenInfoIndex = await GetIndexById<TokenInfoIndex>(tokenId);
        tokenInfoIndex.ShouldNotBeNull();
        tokenInfoIndex.Id.ShouldBe(tokenId);
    }
}