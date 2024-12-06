using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class TokenCreatedLogEventProcessor : TokenProcessorBase<TokenCreated>
{ 
    public override async Task ProcessAsync(TokenCreated logEvent, LogEventContext context)
    {
        Logger.LogInformation("[TokenCreated] handle chainId {chainId} symbol {symbol} externalInfo {externalInfo}",
            context.ChainId, logEvent.Symbol, logEvent.ExternalInfo);
        await TokenInfoIndexCreateAsync(logEvent, context);
    }
}