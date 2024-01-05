using AElf;
using AElf.Contracts.Ewell;
using AElf.Contracts.Whitelist;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public sealed class WhitelistLogEventProcessorTests : EwellIndexerPluginTestBase
{
    
    [Fact]
    public async Task HandleNewWhitelistIdSetAsync_Test()
    {
        var whitelistId = HashHelper.ComputeFrom("test1@gmail.com");
        var projectId = Hash.Empty;
        var logEventContext = MockLogEventContext();

        // step1: create blockStateSet
        var blockStateSetKey = await MockBlockState(logEventContext);

        // step2: create logEventInfo
        var newWhitelistIdSet = new NewWhitelistIdSet
        {
            WhitelistId = whitelistId,
            ProjectId = projectId,
        };
        var logEventInfo = MockLogEventInfo(newWhitelistIdSet.ToLogEvent());
        
        // step3 call the logic
        var newWhitelistIdSetLogEventProcessor = GetRequiredService<NewWhitelistIdSetLogEventProcessor>();
        await newWhitelistIdSetLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        
        // step4 save data after logic
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
    }
    
    [Fact]
    public async Task HandleWhitelistDisableAsync_Test()
    {
        var whitelistId = HashHelper.ComputeFrom("test1@gmail.com");
        var logEventContext = MockLogEventContext();
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistDisabled = new WhitelistDisabled
        {
            WhitelistId = whitelistId,
            IsAvailable = false,
        };
        var logEventInfo = MockLogEventInfo(whitelistDisabled.ToLogEvent());
        var whitelistDisabledProcessor = GetRequiredService<WhitelistDisabledLogEventProcessor>();
        await whitelistDisabledProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
    }
    
    [Fact]
    public async Task HandleWhitelistReenableAsync_Test()
    {
        var whitelistId = HashHelper.ComputeFrom("test1@gmail.com");
        var logEventContext = MockLogEventContext();
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistReenable = new WhitelistReenable()
        {
            WhitelistId = whitelistId,
            IsAvailable = true,
        };
        var logEventInfo = MockLogEventInfo(whitelistReenable.ToLogEvent());
        var whitelistReenableProcessor = GetRequiredService<WhitelistReenableLogEventProcessor>();
        await whitelistReenableProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
    }
}