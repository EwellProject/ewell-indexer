using AElf.Contracts.MultiToken;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using EwellIndexer.Entities;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class TokenIssuedEventProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var symbol = "READ-1";
        await TokenCreatedLogEventProcessor.ProcessAsync(GenerateLogEventContext(TokenCreated(symbol)));
        //issue before
        var address = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(ChainId, symbol); 
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, ChainId, nftIndexId);
        var userBalanceBefore = await GetIndexById<UserBalanceIndex>(userBalanceId);
        var issuedAmount = 1;
        await TokenIssuedEventProcessor.ProcessAsync(GenerateLogEventContext(Issued(symbol, issuedAmount)));
 
        var userBalanceIndex = await GetIndexById<UserBalanceIndex>(userBalanceId);
        userBalanceIndex.ShouldNotBeNull();
        var resultAmount = (userBalanceBefore?.Amount ?? 0) + issuedAmount;
        userBalanceIndex.Amount.ShouldBe(resultAmount);
    }
}