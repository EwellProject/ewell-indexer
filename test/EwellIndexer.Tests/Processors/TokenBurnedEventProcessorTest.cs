using AElf.Types;
using EwellIndexer.Entities;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class TokenBurnedEventProcessorTest : EwellIndexerTestBase
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
        var burnedAmount = 1;
        await TokenBurnedEventProcessor.ProcessAsync(GenerateLogEventContext(Burned(symbol, burnedAmount)));
        
        //check
        var userBalanceIndex = await GetIndexById<UserBalanceIndex>(userBalanceId);
        userBalanceIndex.ShouldNotBeNull();
        var resultAmount = (userBalanceBefore?.Amount ?? 0) - burnedAmount;
        userBalanceIndex.Amount.ShouldBe(resultAmount);
    }
}