using AElf.Contracts.Whitelist;
using AElf.CSharp.Core.Extension;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.Processors;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class WhitelistQueryTest : QueryTestBase
{
    private async Task Init()
    {
        var logEventContext = MockLogEventContext();
        var blockStateSetKey = await MockBlockState(logEventContext);
        var newWhitelistIdSet = new WhitelistDisabled
        {
            WhitelistId = whitelistId,
            IsAvailable = false
        };
        var whitelistDisabledLogEventProcessor = GetRequiredService<WhitelistDisabledLogEventProcessor>();
        var logEventInfo = MockLogEventInfo(newWhitelistIdSet.ToLogEvent());
        await whitelistDisabledLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
    }

    [Fact]
    public async Task WhitelistAsync_Test()
    {
        await Init();
        var whitelistPageResult = await Query.WhiteListAsync(_whitelistRepository, _objectMapper,
            new GetWhiteListDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 100
            });
        whitelistPageResult.ShouldNotBeNull();
        whitelistPageResult.TotalCount.ShouldBe(1);
        whitelistPageResult.Data.Count.ShouldBe(1);
        var whitelistInfoIndex = whitelistPageResult.Data[0];
        whitelistInfoIndex.Id.ShouldBe(whitelistId.ToHex());
        whitelistInfoIndex.ChainId.ShouldBe("tDVW");
        whitelistInfoIndex.IsAvailable.ShouldBe(false);
        
        whitelistPageResult = await Query.WhiteListAsync(_whitelistRepository, _objectMapper,
            new GetWhiteListDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 101
            });
        whitelistPageResult.ShouldNotBeNull();
        whitelistPageResult.TotalCount.ShouldBe(0);
        
        whitelistPageResult = await Query.WhiteListAsync(_whitelistRepository, _objectMapper,
            new GetWhiteListDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 100,
                SkipCount = 1
            });
        whitelistPageResult.ShouldNotBeNull();
        whitelistPageResult.TotalCount.ShouldBe(1);
        whitelistPageResult.Data.Count.ShouldBe(0);
    }

    [Fact]
    public async Task WhitelistByWhitelistIdAsync_Test()
    {
        await Init();
        var whitelistResult = await Query.WhitelistByWhitelistIdAsync(_whitelistRepository, _objectMapper,
            new GetWhiteListDto
            {
                ChainId = "tDVW",
                WhitelistId = whitelistId.ToHex()
            });
        whitelistResult.ShouldNotBeNull();
        whitelistResult.IsAvailable.ShouldBe(false);
        whitelistResult.Id.ShouldBe(whitelistId.ToHex());
        
        whitelistResult = await Query.WhitelistByWhitelistIdAsync(_whitelistRepository, _objectMapper,
            new GetWhiteListDto
            {
                ChainId = "tDVW",
                WhitelistId = "OTHER"
            });
        whitelistResult.ShouldNotBeNull();
        whitelistResult.IsAvailable.ShouldBe(true);
        whitelistResult.Id.ShouldBe("OTHER");
    }
}