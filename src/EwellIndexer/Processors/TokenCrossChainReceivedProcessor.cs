using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class TokenCrossChainReceivedProcessor : TokenProcessorBase<CrossChainReceived>
{
    public override async Task ProcessAsync(CrossChainReceived logEvent, LogEventContext context)
    {
        Logger.LogInformation("[CrossChainReceived] handle chainId {chainId} address {address} amount {amount}", context.ChainId,
            logEvent.To?.ToBase58(), logEvent.Amount);
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To?.ToBase58(),
            logEvent.Amount,
            context);
    }
}