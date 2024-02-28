using AElf.Contracts.MultiToken;
using AElfIndexer.Client.Handlers;
using Ewell.Indexer.Plugin.Processors.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class TokenTransferredLogEventProcessor : TokenProcessorBase<Transferred>
{
    public TokenTransferredLogEventProcessor(ILogger<TokenTransferredLogEventProcessor> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IUserBalanceProvider userBalanceProvider
    ) : base(logger, objectMapper, contractInfoOptions, userBalanceProvider)
    {
    }

    protected override async Task HandleEventAsync(Transferred eventValue, LogEventContext context)
    {
        Logger.LogInformation("[Transferred] handle chainId {chainId} address {address} amount {amount}",
            context.ChainId,
            eventValue.From?.ToBase58(), eventValue.Amount);
        await UserBalanceProvider.SaveUserBalanceAsync(eventValue.Symbol, eventValue.From?.ToBase58(),
            -eventValue.Amount, context);
        
        Logger.LogInformation("[Transferred] handle chainId {chainId} to address {address} amount {amount}",
            context.ChainId,
            eventValue.To?.ToBase58(), eventValue.Amount);
        await UserBalanceProvider.SaveUserBalanceAsync(eventValue.Symbol, eventValue.To?.ToBase58(),
            eventValue.Amount, context);
    }
}