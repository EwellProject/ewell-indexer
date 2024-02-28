using AElf.Contracts.MultiToken;
using AElfIndexer.Client.Handlers;
using Ewell.Indexer.Plugin.Processors.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class TokenIssuedEventProcessor : TokenProcessorBase<Issued>
{
    public TokenIssuedEventProcessor(ILogger<TokenIssuedEventProcessor> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IUserBalanceProvider userBalanceProvider
    ) : base(logger, objectMapper, contractInfoOptions, userBalanceProvider)
    {
    }

    protected override async Task HandleEventAsync(Issued eventValue, LogEventContext context)
    {
        Logger.LogInformation("[Issued] handle chainId {chainId} address {address} amount {amount}", context.ChainId,
            eventValue.To?.ToBase58(), eventValue.Amount);
        await UserBalanceProvider.SaveUserBalanceAsync(eventValue.Symbol, eventValue.To?.ToBase58(),
            eventValue.Amount, context);
    }
}