using AElf;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public sealed class ClaimDamageLogEventProcessorTests : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;

    public ClaimDamageLogEventProcessorTests()
    {
        _crowdfundingProjectRepository =  GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();;
        _userRecordRepository = GetRequiredService<IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo>>();
    }
    
    [Fact]
    public async Task HandleLiquidatedDamageClaimedAsync_Test()
    {
        var projectId = HashHelper.ComputeFrom(Id);
        await MockProjectRegistered();
        await MockInvested();
        await MockDisinvest();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId.ToHex(), Chain_AELF);
        var receivableLiquidatedDamageAmount = projectIndex.ReceivableLiquidatedDamageAmount;
        
        var user = Address.FromPublicKey("AAA".HexToByteArray());
        const long amount = 1L;
        const string investSymbol = "investSymbol";
        var logEventContext = MockLogEventContext(blockHeight, Chain_AELF);
        var blockStateSetKey = await MockBlockState(logEventContext);
        var liquidatedDamageClaimed = new LiquidatedDamageClaimed
        {
            ProjectId = projectId,
            User = user,
            Amount = amount,
            InvestSymbol = investSymbol
        };
        var logEventInfo = MockLogEventInfo(liquidatedDamageClaimed.ToLogEvent());
        var claimDamageLogEventProcessor = GetRequiredService<ClaimDamageLogEventProcessor>();
        await claimDamageLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        
        // normal test
        projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId.ToHex(), logEventContext.ChainId);
        projectIndex.ShouldNotBeNull();
        projectIndex.ReceivableLiquidatedDamageAmount.ShouldBe(receivableLiquidatedDamageAmount - amount);
        var userRecordId = IdGenerateHelper.GetId(logEventContext.ChainId, projectId.ToHex(), user.ToBase58(),
            BehaviorType.LiquidatedDamageClaimed, transactionId);
        var userRecord = await _userRecordRepository.GetFromBlockStateSetAsync(userRecordId, logEventContext.ChainId);
        userRecord.ShouldNotBeNull();
        userRecord.BehaviorType.ShouldBe(BehaviorType.LiquidatedDamageClaimed);
        
        // try-catch
        liquidatedDamageClaimed.User = null;
        logEventInfo = MockLogEventInfo(liquidatedDamageClaimed.ToLogEvent());
        await claimDamageLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);

        // errorProjectId
        var errorProjectId = HashHelper.ComputeFrom("ERROR");
        liquidatedDamageClaimed.ProjectId = errorProjectId;
        logEventInfo = MockLogEventInfo(liquidatedDamageClaimed.ToLogEvent());
        await claimDamageLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(errorProjectId.ToHex(), logEventContext.ChainId);
        projectIndex.ShouldBeNull();
        
        // contract address
        var address = claimDamageLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe("ewellContractAddress");
    }
    
}