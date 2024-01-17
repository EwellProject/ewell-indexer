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

public class TokenIssuedEventProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo>
        _userBalanceIndexRepo;

    public TokenIssuedEventProcessorTest()
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
        //issue before
        var address = Address.FromPublicKey("BBB".HexToByteArray()).ToBase58();
        var nftIndexId = IdGenerateHelper.GetId(chainId, symbol); 
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, chainId, nftIndexId);
        var userBalanceBefore = 
            await _userBalanceIndexRepo.GetFromBlockStateSetAsync(userBalanceId, chainId);
        var issuedAmount = 1;
        var nftIssueLogEventProcessor = GetRequiredService<TokenIssuedEventProcessor>();
        nftIssueLogEventProcessor.GetContractAddress(chainId);
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        var issued = new Issued()
        {
            Symbol = symbol,
            Amount = issuedAmount,
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

        await nftIssueLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);

        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
 
        var userBalanceIndex = await _userBalanceIndexRepo.GetFromBlockStateSetAsync(userBalanceId, chainId);
        userBalanceIndex.ShouldNotBeNull();
        var resultAmount = (userBalanceBefore?.Amount ?? 0) + issuedAmount;
        userBalanceIndex.Amount.ShouldBe(resultAmount);
    }
}