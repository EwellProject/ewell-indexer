using System;
using System.Linq;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using EwellIndexer.Entities;

namespace EwellIndexer.Processors.Base;

public abstract class TokenProcessorBase<TEvent> : ProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    public override string GetContractAddress(string chainId)
    {
        return chainId switch
        {
            EwellConst.TestNetSideChainId => EwellConst.TokenContractAddressTestNetSideChain,
            EwellConst.MainNetSideChainId => EwellConst.TokenContractAddressMainNetSideChain,
            _ => string.Empty
        };
    }
    
    protected async Task SaveUserBalanceAsync(string symbol, string address, long amount, LogEventContext context)
    {
        if (address.IsNullOrWhiteSpace())
        {
            Logger.LogInformation("Save User Balance, Address is null");
            return;
        }
        var nftInfoIndexId = IdGenerateHelper.GetNFTInfoId(context.ChainId, symbol);
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, context.ChainId, nftInfoIndexId);
        var userBalanceIndex = await GetEntityAsync<UserBalanceIndex>(userBalanceId);
        if (userBalanceIndex == null)
        {
            userBalanceIndex = new UserBalanceIndex()
            {
                Id = userBalanceId,
                Address = address,
                Amount = amount,
                Symbol = symbol,
                ChangeTime = context.Block.BlockTime
            };
        }
        else
        {
            userBalanceIndex.Amount += amount;
            userBalanceIndex.ChangeTime = context.Block.BlockTime;
        }

        Logger.LogInformation("SaveUserBalanceAsync Address {Address} symbol {Symbol} balance {Balance}", address,
            symbol, userBalanceIndex.Amount);
        await SaveEntityAsync(userBalanceIndex);
    }
    
    public async Task TokenInfoIndexCreateAsync(TokenCreated eventValue, LogEventContext context)
    {
        if (eventValue == null || context == null)
        {
            return;
        }
        var tokenInfoIndex = ObjectMapper.Map<TokenCreated, TokenInfoIndex>(eventValue);
        tokenInfoIndex.ExternalInfoDictionary = eventValue.ExternalInfo.Value
            .Select(entity => new ExternalInfoDictionary
            {
                Key = entity.Key,
                Value = entity.Value
            }).ToList();
        tokenInfoIndex.Owner = (eventValue.Owner ?? eventValue.Issuer).ToBase58();
        tokenInfoIndex.Issuer = eventValue.Issuer.ToBase58();
    
        tokenInfoIndex.Id = IdGenerateHelper.GetTokenInfoId(context.ChainId, eventValue.Symbol);
        tokenInfoIndex.CreateTime = context.Block.BlockTime;

        await SaveEntityAsync(tokenInfoIndex);
    }
}

