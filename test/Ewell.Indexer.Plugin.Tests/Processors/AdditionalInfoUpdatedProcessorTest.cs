using AElf;
using AElf.CSharp.Core.Extension;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class AdditionalInfoUpdatedProcessorTest : EwellIndexerPluginTestBase
{

    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    public AdditionalInfoUpdatedProcessorTest()
    {
        _crowdfundingProjectRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockProjectRegistered();
        
        var newAdditionalInfo = new AdditionalInfo
        {
            Data =
            {
                { "1", "for value1" }, { "2", "for value2" }
            }
        };
        await MockAdditionalInfoUpdated(newAdditionalInfo);

        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, Chain_AELF);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        
        var newAdditionalInfoDic =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(projectIndex.AdditionalInfo);
        newAdditionalInfoDic?.Count.ShouldBe(2);
        newAdditionalInfoDic?["1"].ShouldBe("for value1");
        newAdditionalInfoDic?["2"].ShouldBe("for value2");
    }

    private async Task MockAdditionalInfoUpdated(AdditionalInfo additionalInfo)
    {
        string chainId = Chain_AELF;
        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var logEvent = new AdditionalInfoUpdated()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            
            AdditionalInfo = additionalInfo
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;

        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow
        };

        var processor = GetRequiredService<AdditionalInfoUpdatedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
    }
}