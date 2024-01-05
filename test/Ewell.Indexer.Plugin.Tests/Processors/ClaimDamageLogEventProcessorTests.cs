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
        
        
    }
}