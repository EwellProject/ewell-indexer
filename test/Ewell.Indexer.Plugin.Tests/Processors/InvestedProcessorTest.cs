using AElf;
using AElf.Contracts.Ewell;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class InvestedProcessorTest : EwellIndexerPluginTestBase
{
    
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;


    public InvestedProcessorTest()
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
        var logEvent = new Invested()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            Amount = 1000,
            TotalAmount = 1000,
            ToClaimAmount = 2000
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
            TransactionId = transactionId
        };
        
        var processor = GetRequiredService<InvestedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(chainId, logEvent.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, chainId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.InvestAmount.ShouldBe(logEvent.Amount);
    }
}