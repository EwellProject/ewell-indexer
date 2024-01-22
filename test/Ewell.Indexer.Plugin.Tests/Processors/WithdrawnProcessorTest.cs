using AElf;
using AElf.Contracts.Ewell;
using AElf.CSharp.Core.Extension;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class WithdrawnProcessorTest : EwellIndexerPluginTestBase
{
    
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;


    public WithdrawnProcessorTest()
    {
        _crowdfundingProjectRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
        _userProjectInfoRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo>>();
        _userRecordRepository = GetRequiredService<IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockProjectRegistered();

        await MockWithdrawn();
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, Chain_AELF);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.IsWithdraw.ShouldBeTrue();
    }

    private async Task MockWithdrawn()
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
        var logEvent = new Withdrawn()
        {
            ProjectId = HashHelper.ComputeFrom(Id)
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

        var processor = GetRequiredService<WithdrawnProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
    }
}