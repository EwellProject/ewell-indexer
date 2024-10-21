using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class TokenBurnedEventProcessor : TokenProcessorBase<Burned>
{
    public override async Task ProcessAsync(Burned logEvent, LogEventContext context)
    {
        Logger.LogInformation("[Burned] handle chainId {chainId} address {address} amount {amount}", context.ChainId,
            logEvent.Burner?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.Burner?.ToBase58(), -logEvent.Amount, context);
    }
}