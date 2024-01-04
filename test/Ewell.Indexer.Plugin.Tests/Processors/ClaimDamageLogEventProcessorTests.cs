using AElf;
using AElf.Contracts.Ewell;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public sealed class ClaimDamageLogEventProcessorTests : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo> _liquidatedDamageClaimedIndexRepository;

    public ClaimDamageLogEventProcessorTests()
    {
        _liquidatedDamageClaimedIndexRepository = GetRequiredService<IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleLiquidatedDamageClaimedAsync_Test()
    {
        var projectId = HashHelper.ComputeFrom("project");
        var user = Address.FromPublicKey("AAA".HexToByteArray());
        var id = IdGenerateHelper.GetId(projectId.ToHex(), user.ToBase58());
        const long amount = 1L;
        const string investSymbol = "investSymbol";
        var logEventContext = MockLogEventContext();
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
        
        var liquidatedDamageClaimedIndexInfo = await _liquidatedDamageClaimedIndexRepository.GetFromBlockStateSetAsync(id, logEventContext.ChainId);
        liquidatedDamageClaimedIndexInfo.ShouldNotBeNull();
        liquidatedDamageClaimedIndexInfo.BlockHeight.ShouldBe(logEventContext.BlockHeight);
        liquidatedDamageClaimedIndexInfo.BlockHash.ShouldBe(logEventContext.BlockHash);
        liquidatedDamageClaimedIndexInfo.Id.ShouldBe(id);
        liquidatedDamageClaimedIndexInfo.ChainId.ShouldBe(logEventContext.ChainId);
        liquidatedDamageClaimedIndexInfo.User.ShouldBe(user.ToBase58());
        liquidatedDamageClaimedIndexInfo.ProjectId.ShouldBe(projectId.ToHex());
        liquidatedDamageClaimedIndexInfo.InvestSymbol.ShouldBe(investSymbol);
        liquidatedDamageClaimedIndexInfo.Amount.ShouldBe(amount);
        
        await claimDamageLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        liquidatedDamageClaimedIndexInfo = await _liquidatedDamageClaimedIndexRepository.GetFromBlockStateSetAsync(id, logEventContext.ChainId);
        liquidatedDamageClaimedIndexInfo.Amount.ShouldBe(amount);
    }
}