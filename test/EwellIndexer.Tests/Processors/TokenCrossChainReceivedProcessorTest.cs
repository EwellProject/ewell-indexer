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

public class TokenCrossChainReceivedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var symbol = "READ-1";
        await TokenCreatedLogEventProcessor.ProcessAsync(GenerateLogEventContext(TokenCreated(symbol)));
        
        var address = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(ChainId, symbol); 
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, ChainId, nftIndexId);
        var userBalanceBefore = await GetIndexById<UserBalanceIndex>(userBalanceId);
        var transferAmount = 1;
        await TokenCrossChainReceivedProcessor.ProcessAsync(GenerateLogEventContext(CrossChainReceived(symbol, transferAmount)));
        
        //check
        var userBalanceIndex = await GetIndexById<UserBalanceIndex>(userBalanceId);
        userBalanceIndex.ShouldNotBeNull();
        var resultAmount = (userBalanceBefore?.Amount ?? 0) + transferAmount;
        userBalanceIndex.Amount.ShouldBe(resultAmount);
    }
}