using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class TokenCreatedLogEventProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<TokenInfoIndex, LogEventInfo>
        _tokenIndexRepo;
    
    public TokenCreatedLogEventProcessorTest()
    {
        _tokenIndexRepo =
            GetRequiredService<IAElfIndexerClientEntityRepository<TokenInfoIndex, LogEventInfo>>();
    }
    
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var chainId = Chain_AELF;
        var symbol = "READ-1";
        await MockTokenCreated(chainId, symbol);
        var tokenId = IdGenerateHelper.GetTokenInfoId(chainId, symbol);
        var tokenInfoIndex = await _tokenIndexRepo.GetFromBlockStateSetAsync(tokenId, Chain_AELF);
        tokenInfoIndex.ShouldNotBeNull();
        tokenInfoIndex.Id.ShouldBe(tokenId);
    }
}