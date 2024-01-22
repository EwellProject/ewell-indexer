using AElf;
using AElf.Contracts.Ewell;
using AElf.Contracts.Whitelist;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using ExtraInfoId = AElf.Contracts.Whitelist.ExtraInfoId;
using ExtraInfoIdList = AElf.Contracts.Whitelist.ExtraInfoIdList;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public sealed class WhitelistLogEventProcessorTests : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;
    private readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo>
        _whitelistRepository;

    public WhitelistLogEventProcessorTests()
    {
        _whitelistRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo>>();
        _crowdfundingProjectRepository =  GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleNewWhitelistIdSetAsync_Test()
    {
        await MockProjectRegistered();
        var projectId = HashHelper.ComputeFrom(Id);
        var logEventContext = MockLogEventContext(blockHeight, Chain_AELF);

        // step1: create blockStateSet
        var blockStateSetKey = await MockBlockState(logEventContext);

        // step2: create logEventInfo
        var newWhitelistIdSet = new NewWhitelistIdSet
        {
            WhitelistId = WhitelistId,
            ProjectId = projectId,
        };
        var logEventInfo = MockLogEventInfo(newWhitelistIdSet.ToLogEvent());
        
        // step3 call the logic
        var newWhitelistIdSetLogEventProcessor = GetRequiredService<NewWhitelistIdSetLogEventProcessor>();
        await newWhitelistIdSetLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        
        // step4 save data after logic
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        // normal test
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(ProjectId, logEventContext.ChainId);
        projectIndex.ShouldNotBeNull();
        projectIndex.WhitelistId.ShouldBe(WhitelistId.ToHex());
        
        // try-catch
        newWhitelistIdSet.WhitelistId = null;
        logEventInfo = MockLogEventInfo(newWhitelistIdSet.ToLogEvent());
        await newWhitelistIdSetLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);

        // errorProjectId
        var errorProjectId = HashHelper.ComputeFrom("ERROR");
        newWhitelistIdSet.ProjectId = errorProjectId;
        logEventInfo = MockLogEventInfo(newWhitelistIdSet.ToLogEvent());
        await newWhitelistIdSetLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(errorProjectId.ToHex(), logEventContext.ChainId);
        projectIndex.ShouldBeNull();
        
        // contract address
        var address = newWhitelistIdSetLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe("whitelist");
    }
    
    [Fact]
    public async Task HandleWhitelistDisableAsync_Test()
    {
        var logEventContext = MockLogEventContext(120, Chain_AELF);
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistDisabled = new WhitelistDisabled
        {
            WhitelistId = WhitelistId,
            IsAvailable = false,
        };
        var logEventInfo = MockLogEventInfo(whitelistDisabled.ToLogEvent());
        var whitelistDisabledProcessor = GetRequiredService<WhitelistDisabledLogEventProcessor>();
        await whitelistDisabledProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        var whitelist = await _whitelistRepository.GetFromBlockStateSetAsync(WhitelistId.ToHex(), logEventContext.ChainId);
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(false);
        
        // contract address
        var address = whitelistDisabledProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe("whitelist");
    }
    
    [Fact]
    public async Task HandleWhitelistReenableAsync_Test()
    {
        var logEventContext = MockLogEventContext(blockHeight, Chain_AELF);
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistReenable = new WhitelistReenable
        {
            WhitelistId = WhitelistId,
            IsAvailable = true,
        };
        var logEventInfo = MockLogEventInfo(whitelistReenable.ToLogEvent());
        var whitelistReenableProcessor = GetRequiredService<WhitelistReenableLogEventProcessor>();
        await whitelistReenableProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        // normal test
        var whitelist = await _whitelistRepository.GetFromBlockStateSetAsync(WhitelistId.ToHex(), logEventContext.ChainId);
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(true);
        
        // contract address
        var address = whitelistReenableProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe("whitelist");
    }

    [Fact]
    public async Task HandleWhitelistAddressInfoAddedAsync_Test()
    {
        await HandleWhitelistReenableAsync_Test();
        var logEventContext = MockLogEventContext(blockHeight, Chain_AELF);
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistAddressInfoAdded = new WhitelistAddressInfoAdded
        {
            WhitelistId = WhitelistId,
            ExtraInfoIdList = new ExtraInfoIdList
            {
                Value = { new []
                {
                    new ExtraInfoId
                    {
                        AddressList = new AddressList
                        {
                            Value = { Address.FromBase58(AliceAddress) }
                        }
                    }
                }}
            }
        };
        var logEventInfo = MockLogEventInfo(whitelistAddressInfoAdded.ToLogEvent());
        var whitelistAddressInfoAddedProcessor = GetRequiredService<WhitelistAddressInfoAddedProcessor>();
        await whitelistAddressInfoAddedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        //normal test
        var whitelist = await _whitelistRepository.GetFromBlockStateSetAsync(WhitelistId.ToHex(), logEventContext.ChainId);
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(true);
        var whitelistAddressTimeInfo = whitelist.AddressTimeInfo;
        whitelistAddressTimeInfo.ShouldNotBeNull();
        var whitelistAddressTime = JsonConvert.DeserializeObject<WhitelistAddressTime>(whitelistAddressTimeInfo);
        whitelistAddressTime.ShouldNotBeNull();
        whitelistAddressTime.Address.ShouldBe(Address.FromBase58(AliceAddress).ToBase58());
        whitelistAddressTime.CreateTime.ShouldBe(BlockTime);
    }
    
    [Fact]
    public async Task HandleWhitelistAddressInfoRemovedAsync_Test()
    {
        await HandleWhitelistAddressInfoAddedAsync_Test();
        var logEventContext = MockLogEventContext(blockHeight, Chain_AELF);
        var blockStateSetKey = await MockBlockState(logEventContext);
        var whitelistAddressInfoRemoved = new WhitelistAddressInfoRemoved
        {
            WhitelistId = WhitelistId,
            ExtraInfoIdList = new ExtraInfoIdList
            {
                Value = { new []
                {
                    new ExtraInfoId
                    {
                        AddressList = new AddressList
                        {
                            Value = { Address.FromBase58(AliceAddress) }
                        }
                    }
                }}
            }
        };
        var logEventInfo = MockLogEventInfo(whitelistAddressInfoRemoved.ToLogEvent());
        var whitelistAddressInfoRemovedProcessor = GetRequiredService<WhitelistAddressInfoRemovedProcessor>();
        await whitelistAddressInfoRemovedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        //normal test
        var whitelist = await _whitelistRepository.GetFromBlockStateSetAsync(WhitelistId.ToHex(), logEventContext.ChainId);
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(true);
        whitelist.AddressTimeInfo.ShouldBeNull();
    }
}