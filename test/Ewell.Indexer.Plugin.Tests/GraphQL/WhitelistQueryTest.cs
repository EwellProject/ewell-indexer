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
    }
}