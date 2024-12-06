using AeFinder.App.TestBase;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using EwellIndexer.Entities;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class TokenTransferredLogEventProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var symbol = "READ-1";
        await TokenCreatedLogEventProcessor.ProcessAsync(GenerateLogEventContext(TokenCreated(symbol)));
        
        var fromAddress = Address.FromPublicKey("AAA".HexToByteArray()).ToBase58();
        var toAddress = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(ChainId, symbol); 
        var fromUserBalanceId = IdGenerateHelper.GetUserBalanceId(fromAddress, ChainId, nftIndexId);
        var toUserBalanceId = IdGenerateHelper.GetUserBalanceId(toAddress, ChainId, nftIndexId);
        var fromUserBalanceBefore = await GetIndexById<UserBalanceIndex>(fromUserBalanceId);
        var toUserBalanceBefore = await GetIndexById<UserBalanceIndex>(toUserBalanceId);
        var transferAmount = 1;
        
        await TokenTransferredLogEventProcessor.ProcessAsync(GenerateLogEventContext(Transferred(symbol, transferAmount)));
        
        //check
        var fromUserBalanceIndex = await GetIndexById<UserBalanceIndex>(fromUserBalanceId);
        fromUserBalanceIndex.ShouldNotBeNull();
        var fromResultAmount = (fromUserBalanceBefore?.Amount ?? 0) - transferAmount;
        fromUserBalanceIndex.Amount.ShouldBe(fromResultAmount);
        
        var toUserBalanceIndex = await GetIndexById<UserBalanceIndex>(toUserBalanceId);
        toUserBalanceIndex.ShouldNotBeNull();
        var toResultAmount = (toUserBalanceBefore?.Amount ?? 0) + transferAmount;
        toUserBalanceIndex.Amount.ShouldBe(toResultAmount);
    }
}