using AElf.Contracts.MultiToken;
using AElfIndexer.Client.Handlers;
using Ewell.Indexer.Plugin.Processors.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class TokenBurnedEventProcessor : TokenProcessorBase<Burned>
{
    public TokenBurnedEventProcessor(ILogger<TokenBurnedEventProcessor> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IUserBalanceProvider userBalanceProvider
    ) : base(logger, objectMapper, contractInfoOptions, userBalanceProvider)
    {
    }

    protected override async Task HandleEventAsync(Burned eventValue, LogEventContext context)
    {
        Logger.LogInformation("[Burned] handle chainId {chainId} address {address} amount {amount}", context.ChainId,
            eventValue.Burner?.ToBase58(), eventValue.Amount);
        await UserBalanceProvider.SaveUserBalanceAsync(eventValue.Symbol,
            eventValue.Burner?.ToBase58(), -eventValue.Amount, context);
    }
}