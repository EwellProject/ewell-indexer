using AElf;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class ClaimedProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;


    public ClaimedProcessorTest()
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
        var logEvent = new Claimed()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            ProjectSymbol = TestSymbol,
            Amount = 2000,
            TotalPeriod = 1,
            LatestPeriod = 1
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

        var processor = GetRequiredService<ClaimedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, chainId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.CurrentRaisedAmount.ShouldBe(invested.Amount);
        projectIndex.CurrentCrowdFundingIssueAmount.ShouldBe(invested.ToClaimAmount - logEvent.Amount);
        
        var userRecordId = IdGenerateHelper.GetId(chainId, projectId, BobAddress, 
            BehaviorType.Claim, transactionId);
        var userRecordIndex = await _userRecordRepository.GetFromBlockStateSetAsync(userRecordId, chainId);
        userRecordIndex.ShouldNotBeNull();
        userRecordIndex.Id.ShouldBe(userRecordId);
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(chainId, logEvent.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, chainId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.ActualClaimAmount.ShouldBe(logEvent.Amount);
        userProjectInfoIndex.ToClaimAmount.ShouldBe(invested.ToClaimAmount - logEvent.Amount);
    }
}