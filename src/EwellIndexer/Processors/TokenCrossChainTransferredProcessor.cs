using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class TokenCrossChainTransferredProcessor : TokenProcessorBase<CrossChainTransferred>
{
    public override async Task ProcessAsync(CrossChainTransferred logEvent, LogEventContext context)
    {
        Logger.LogInformation("[CrossChainTransferred] handle chainId {chainId} from address {address} amount {amount}",
            context.ChainId,
            logEvent.From?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.From?.ToBase58(),
            -logEvent.Amount, context);
        
        Logger.LogInformation("[CrossChainTransferred] handle chainId {chainId} to address {address} amount {amount}",
            context.ChainId,
            logEvent.To?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To?.ToBase58(),
            logEvent.Amount, context);
    }
}