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
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class RefundedProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;


    public RefundedProcessorTest()
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

        var invested = await MockInvested();

        var chainId = Chain_AELF;

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
        var logEvent = new ReFunded()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            InvestSymbol = TestSymbol,
            Amount = 1000
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

        var processor = GetRequiredService<RefundedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);

        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var userRecordId = IdGenerateHelper.GetId(chainId, projectId, BobAddress,
            BehaviorType.Refund, transactionId);
        var userRecordIndex = await _userRecordRepository.GetFromBlockStateSetAsync(userRecordId, chainId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
        userRecordIndex.ToRaiseTokenAmount.ShouldBe(logEvent.Amount);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(chainId, logEvent.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, chainId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.InvestAmount.ShouldBe(invested.Amount - logEvent.Amount);
        userProjectInfoIndex.ToClaimAmount.ShouldBe(0);
    }
}
