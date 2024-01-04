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
    private readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistIndexRepository;
    private readonly IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> _projectIndexRepository;

    public WhitelistLogEventProcessorTests()
    {
        _whitelistIndexRepository = GetRequiredService<IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo>>();
        _projectIndexRepository = GetRequiredService<IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo>>();
    }
    
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
        
        var projectIndex = await _projectIndexRepository.GetFromBlockStateSetAsync(projectId.ToHex(), logEventContext.ChainId);
        projectIndex.ShouldNotBeNull();
        projectIndex.BlockHeight.ShouldBe(logEventContext.BlockHeight);
        projectIndex.BlockHash.ShouldBe(logEventContext.BlockHash);
        projectIndex.ChainId.ShouldBe(logEventContext.ChainId);
        projectIndex.Id.ShouldBe(projectId.ToHex());
        projectIndex.WhitelistId.ShouldBe(whitelistId.ToHex());

        var whitelistId2 = HashHelper.ComputeFrom("test2@gmail.com");
        newWhitelistIdSet.WhitelistId = whitelistId2;
        await newWhitelistIdSetLogEventProcessor.HandleEventAsync(MockLogEventInfo(newWhitelistIdSet.ToLogEvent()), logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        projectIndex = await _projectIndexRepository.GetFromBlockStateSetAsync(projectId.ToHex(), logEventContext.ChainId);
        projectIndex.WhitelistId.ShouldBe(whitelistId2.ToHex());
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
        var whitelistInfoIndexData = await _whitelistIndexRepository.GetFromBlockStateSetAsync(whitelistId.ToHex(), logEventContext.ChainId);
        whitelistInfoIndexData.ShouldNotBeNull();
        whitelistInfoIndexData.BlockHeight.ShouldBe(logEventContext.BlockHeight);
        whitelistInfoIndexData.BlockHash.ShouldBe(logEventContext.BlockHash);
        whitelistInfoIndexData.Id.ShouldBe(whitelistId.ToHex());
        whitelistInfoIndexData.IsAvailable.ShouldBe(false);
        
        await whitelistDisabledProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        whitelistInfoIndexData = await _whitelistIndexRepository.GetFromBlockStateSetAsync(whitelistId.ToHex(), logEventContext.ChainId);
        whitelistInfoIndexData.IsAvailable.ShouldBe(false);
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
        var whitelistInfoIndexData = await _whitelistIndexRepository.GetFromBlockStateSetAsync(whitelistId.ToHex(), logEventContext.ChainId);
        whitelistInfoIndexData.ShouldNotBeNull();
        whitelistInfoIndexData.BlockHeight.ShouldBe(logEventContext.BlockHeight);
        whitelistInfoIndexData.BlockHash.ShouldBe(logEventContext.BlockHash);
        whitelistInfoIndexData.Id.ShouldBe(whitelistId.ToHex());
        whitelistInfoIndexData.IsAvailable.ShouldBe(true);

        await HandleWhitelistDisableAsync_Test();
        await whitelistReenableProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        whitelistInfoIndexData = await _whitelistIndexRepository.GetFromBlockStateSetAsync(whitelistId.ToHex(), logEventContext.ChainId);
        whitelistInfoIndexData.IsAvailable.ShouldBe(true);
    }
}