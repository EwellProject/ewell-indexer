using AElf;
using AElf.Contracts.Ewell;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.Processors;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class ClaimedDamageQueryTest : QueryTestBase
{
    [Fact]
    public async Task ClaimedDamageQueryAsync_Test()
    {
        var projectId = HashHelper.ComputeFrom("project");
        var user = Address.FromPublicKey("AAA".HexToByteArray());
        var id = IdGenerateHelper.GetId(projectId.ToHex(), user.ToBase58());
        const long amount = 5L;
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
        
        var claimedDamagePageResult = await Query.ClaimedDamageAsync(_liquidatedDamageClaimedRepository, _objectMapper, 
            new GetDamageClaimedDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 100
            });
        claimedDamagePageResult.ShouldNotBeNull();
        claimedDamagePageResult.TotalCount.ShouldBe(1);
        claimedDamagePageResult.Data.Count.ShouldBe(1);
        var damageClaimedInfoIndexDto = claimedDamagePageResult.Data[0];
        damageClaimedInfoIndexDto.Id.ShouldBe(id);
        damageClaimedInfoIndexDto.BlockHeight.ShouldBe(100);
        damageClaimedInfoIndexDto.ProjectId.ShouldBe(projectId.ToHex());
        damageClaimedInfoIndexDto.InvestSymbol.ShouldBe(investSymbol);
        damageClaimedInfoIndexDto.ChainId.ShouldBe(logEventContext.ChainId);
        damageClaimedInfoIndexDto.User.ShouldBe(user.ToBase58());
        damageClaimedInfoIndexDto.Amount.ShouldBe(5);
        
        claimedDamagePageResult = await Query.ClaimedDamageAsync(_liquidatedDamageClaimedRepository, _objectMapper, 
            new GetDamageClaimedDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 101
            });
        claimedDamagePageResult.ShouldNotBeNull();
        claimedDamagePageResult.TotalCount.ShouldBe(0);
        
        claimedDamagePageResult = await Query.ClaimedDamageAsync(_liquidatedDamageClaimedRepository, _objectMapper, 
            new GetDamageClaimedDto
            {
                ChainId = "tDVW",
                StartBlockHeight = 100,
                SkipCount = 1
            });
        claimedDamagePageResult.ShouldNotBeNull();
        claimedDamagePageResult.TotalCount.ShouldBe(1);
        claimedDamagePageResult.Data.Count.ShouldBe(0);
    }
}