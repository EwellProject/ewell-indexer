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

public class TokenCrossChainTransferredProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo>
        _userBalanceIndexRepo;

    public TokenCrossChainTransferredProcessorTest()
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
        
        var fromAddress = Address.FromPublicKey("AAA".HexToByteArray()).ToBase58();
        var toAddress = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(chainId, symbol); 
        var fromUserBalanceId = IdGenerateHelper.GetUserBalanceId(fromAddress, chainId, nftIndexId);
        var toUserBalanceId = IdGenerateHelper.GetUserBalanceId(toAddress, chainId, nftIndexId);
        var fromUserBalanceBefore = 
            await _userBalanceIndexRepo.GetFromBlockStateSetAsync(fromUserBalanceId, chainId);
        var toUserBalanceBefore = 
            await _userBalanceIndexRepo.GetFromBlockStateSetAsync(toUserBalanceId, chainId);
        var transferAmount = 1;
        
        await MockTokenCrossChainTransferred(chainId, symbol, transferAmount);
        
        //check
        var fromUserBalanceIndex = await _userBalanceIndexRepo.GetFromBlockStateSetAsync(fromUserBalanceId, chainId);
        fromUserBalanceIndex.ShouldNotBeNull();
        var fromResultAmount = (fromUserBalanceBefore?.Amount ?? 0) - transferAmount;
        fromUserBalanceIndex.Amount.ShouldBe(fromResultAmount);
        
        var toUserBalanceIndex = await _userBalanceIndexRepo.GetFromBlockStateSetAsync(toUserBalanceId, chainId);
        toUserBalanceIndex.ShouldNotBeNull();
        var toResultAmount = (toUserBalanceBefore?.Amount ?? 0) + transferAmount;
        toUserBalanceIndex.Amount.ShouldBe(toResultAmount);
    }

    private async Task MockTokenCrossChainTransferred(string chainId, string symbol, long transferAmount)
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
            From = Address.FromPublicKey("AAA".HexToByteArray()),
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

        var logEventProcessor = GetRequiredService<TokenCrossChainTransferredProcessor>();
        logEventProcessor.GetContractAddress(chainId);
        await logEventProcessor.HandleEventAsync(logEventInfo, logEventContext);

        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
    }
}