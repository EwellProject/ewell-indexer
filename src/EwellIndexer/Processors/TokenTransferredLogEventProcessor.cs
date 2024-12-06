using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class TokenTransferredLogEventProcessor : TokenProcessorBase<Transferred>
{
    public override async Task ProcessAsync(Transferred logEvent, LogEventContext context)
    {
        Logger.LogInformation("[Transferred] handle chainId {chainId} address {address} amount {amount}",
            context.ChainId,
            logEvent.From?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.From?.ToBase58(),
            -logEvent.Amount, context);
        
        Logger.LogInformation("[Transferred] handle chainId {chainId} to address {address} amount {amount}",
            context.ChainId,
            logEvent.To?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To?.ToBase58(),
            logEvent.Amount, context);
    }
}