using AElf.Contracts.MultiToken;
using AElfIndexer.Client.Handlers;
using Ewell.Indexer.Plugin.Processors.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class TokenCreatedLogEventProcessor : TokenProcessorBase<TokenCreated>
{
    private readonly ITokenInfoProvider _tokenInfoProvider;

    public TokenCreatedLogEventProcessor(ILogger<TokenCreatedLogEventProcessor> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IUserBalanceProvider userBalanceProvider,
        ITokenInfoProvider tokenInfoProvider) : base(logger, objectMapper, contractInfoOptions, userBalanceProvider)
    {
        _tokenInfoProvider = tokenInfoProvider;
    }

    protected override async Task HandleEventAsync(TokenCreated eventValue, LogEventContext context)
    {
        Logger.LogInformation("[TokenCreated] handle chainId {chainId} symbol {symbol} externalInfo {externalInfo}",
            context.ChainId, eventValue.Symbol, eventValue.ExternalInfo);
        await _tokenInfoProvider.TokenInfoIndexCreateAsync(eventValue, context);
    }
}