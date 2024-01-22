using AElf.Contracts.MultiToken;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class TokenCrossChainReceivedProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo>
        _userBalanceIndexRepo;

    public TokenCrossChainReceivedProcessorTest()
    {
        _userBalanceIndexRepo =
            GetRequiredService<IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        var chainId = Chain_AELF;
        var symbol = "READ-1";
        await MockTokenCreated(chainId, symbol);
        
        var address = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(chainId, symbol); 
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, chainId, nftIndexId);
        var userBalanceBefore = 
            await _userBalanceIndexRepo.GetFromBlockStateSetAsync(userBalanceId, chainId);
        var transferAmount = 1;
        await MockTokenCrossChainReceived(chainId, symbol, transferAmount);
        
        //check
        var userBalanceIndex = await _userBalanceIndexRepo.GetFromBlockStateSetAsync(userBalanceId, chainId);
        userBalanceIndex.ShouldNotBeNull();
        var resultAmount = (userBalanceBefore?.Amount ?? 0) + transferAmount;
        userBalanceIndex.Amount.ShouldBe(resultAmount);
    }

    private async Task MockTokenCrossChainReceived(string chainId, string symbol, long transferAmount)
    {
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        var issued = new CrossChainReceived()
        {
            Symbol = symbol,
            Amount = transferAmount,
            To = Address.FromPublicKey("BBB".HexToByteArray())
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(issued.ToLogEvent());
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
            TransactionId = transactionId
        };

        var logEventProcessor = GetRequiredService<TokenCrossChainReceivedProcessor>();
        logEventProcessor.GetContractAddress(chainId);
        await logEventProcessor.HandleEventAsync(logEventInfo, logEventContext);

        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
    }
}