using AElf;
using AElf.Types;
using EwellIndexer.Entities;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public sealed class ClaimDamageLogEventProcessorTests : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleLiquidatedDamageClaimedAsync_Test()
    {
        var projectId = HashHelper.ComputeFrom(Id);
        var user = Address.FromPublicKey("AAA".HexToByteArray());
        const long amount = 1L;
        const string investSymbol = "investSymbol";
        
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await MockEventProcess(Invested(), InvestedProcessor);
        await MockEventProcess(DisInvested(), DisInvestedProcessor);
       
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId.ToHex());
        var receivableLiquidatedDamageAmount = projectIndex.ReceivableLiquidatedDamageAmount;
        await MockEventProcess(LiquidatedDamageClaimed(projectId, user, amount, investSymbol), ClaimDamageLogEventProcessor);
        
        // normal test
        projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId.ToHex());
        projectIndex.ShouldNotBeNull();
        projectIndex.ReceivableLiquidatedDamageAmount.ShouldBe(receivableLiquidatedDamageAmount - amount);
        var userRecordId = IdGenerateHelper.GetId(ChainId, projectId.ToHex(), user.ToBase58(),
            BehaviorType.LiquidatedDamageClaimed, transactionId);
        var userRecord = await GetIndexById<UserRecordIndex>(userRecordId);
        userRecord.ShouldNotBeNull();
        userRecord.BehaviorType.ShouldBe(BehaviorType.LiquidatedDamageClaimed);
        
        // try-catch
        await MockEventProcess(LiquidatedDamageClaimed(projectId, null, amount, investSymbol), ClaimDamageLogEventProcessor);

        // errorProjectId
        var errorProjectId = HashHelper.ComputeFrom("ERROR");
        await MockEventProcess(LiquidatedDamageClaimed(errorProjectId, null, amount, investSymbol), ClaimDamageLogEventProcessor);
        projectIndex = await GetIndexById<CrowdfundingProjectIndex>(errorProjectId.ToHex());
        projectIndex.ShouldBeNull();
        
        // contract address
        var address = ClaimDamageLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe(EwellConst.EwellContractAddressMainNetSideChain);
    }
    
}