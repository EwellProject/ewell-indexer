using AElf.Contracts.ProxyAccountContract;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ProxyAccountCreatedProcessor : AElfLogEventProcessorBase<ProxyAccountCreated, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<ProxyAccountIndex, LogEventInfo> _proxyAccountIndexRepository;
    private readonly ILogger<AElfLogEventProcessorBase<ProxyAccountCreated, LogEventInfo>> _logger;

    public ProxyAccountCreatedProcessor(
        ILogger<AElfLogEventProcessorBase<ProxyAccountCreated, LogEventInfo>> logger,
        IAElfIndexerClientEntityRepository<ProxyAccountIndex, LogEventInfo> proxyAccountIndexRepository,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions) : base(logger)
    {
        _objectMapper = objectMapper;
        _logger = logger;
        _contractInfoOptions = contractInfoOptions.Value;
        _proxyAccountIndexRepository = proxyAccountIndexRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].ProxyAccountContractAddress;
    }

    protected override async Task HandleEventAsync(ProxyAccountCreated eventValue, LogEventContext context)
    {
        _logger.LogInformation("[ProxyAccountCreated] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, eventValue?.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(eventValue?.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = _objectMapper.Map<ProxyAccountCreated, ProxyAccountIndex>(eventValue);
        proxyAccountIndex.Id = proxyAccountIndexId;
        _objectMapper.Map(context, proxyAccountIndex);
        proxyAccountIndex.CreateTime = context.BlockTime;
        await _proxyAccountIndexRepository.AddOrUpdateAsync(proxyAccountIndex);
    }
}