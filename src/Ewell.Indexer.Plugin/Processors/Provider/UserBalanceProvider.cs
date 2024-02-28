using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors.Provider;

public interface IUserBalanceProvider
{
    public Task<long> SaveUserBalanceAsync(string symbol, string address, long amount, LogEventContext context);
}


public class UserBalanceProvider : IUserBalanceProvider, ISingletonDependency
{
    private readonly IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo> _userBalanceIndexRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<IUserBalanceProvider> _logger;
    
    public UserBalanceProvider(IAElfIndexerClientEntityRepository<UserBalanceIndex, 
        LogEventInfo> userBalanceIndexRepository, IObjectMapper objectMapper, 
        ILogger<IUserBalanceProvider> logger)
    {
        _userBalanceIndexRepository = userBalanceIndexRepository;
        _objectMapper = objectMapper;
        _logger = logger;
    }

    public async Task<long> SaveUserBalanceAsync(string symbol, string address, long amount, LogEventContext context)
    {
        if (address.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("Save User Balance, Address is null");
        }
        var nftInfoIndexId = IdGenerateHelper.GetNFTInfoId(context.ChainId, symbol);
        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, context.ChainId, nftInfoIndexId);
        var userBalanceIndex =
            await _userBalanceIndexRepository.GetFromBlockStateSetAsync(userBalanceId, context.ChainId);
        if (userBalanceIndex == null)
        {
            userBalanceIndex = new UserBalanceIndex()
            {
                Id = userBalanceId,
                ChainId = context.ChainId,
                Address = address,
                Amount = amount,
                Symbol = symbol,
                ChangeTime = context.BlockTime
            };
        }
        else
        {
            userBalanceIndex.Amount += amount;
            userBalanceIndex.ChangeTime = context.BlockTime;
        }

        _objectMapper.Map(context, userBalanceIndex);
        _logger.LogInformation("SaveUserBalanceAsync Address {Address} symbol {Symbol} balance {Balance}", address,
            symbol, userBalanceIndex.Amount);
        await _userBalanceIndexRepository.AddOrUpdateAsync(userBalanceIndex);
        return userBalanceIndex.Amount;
    }
}
