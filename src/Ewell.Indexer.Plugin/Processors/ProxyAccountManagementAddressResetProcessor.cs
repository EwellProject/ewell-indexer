using AElf.Contracts.ProxyAccountContract;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ProxyAccountManagementAddressResetProcessor : AElfLogEventProcessorBase<ProxyAccountManagementAddressReset, LogEventInfo>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<ProxyAccountIndex, LogEventInfo> _proxyAccountIndexRepository;
    private readonly ILogger<AElfLogEventProcessorBase<ProxyAccountManagementAddressReset, LogEventInfo>> _logger;

    public ProxyAccountManagementAddressResetProcessor(
        ILogger<AElfLogEventProcessorBase<ProxyAccountManagementAddressReset, LogEventInfo>> logger,
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

    protected override async Task HandleEventAsync(ProxyAccountManagementAddressReset eventValue,
        LogEventContext context)
    {
        _logger.LogInformation(
            "[ProxyAccountManagementAddressReset] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, eventValue.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(eventValue.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = _objectMapper.Map<ProxyAccountManagementAddressReset, ProxyAccountIndex>(eventValue);
        _objectMapper.Map(context, proxyAccountIndex);
        proxyAccountIndex.Id = proxyAccountIndexId;
        await _proxyAccountIndexRepository.AddOrUpdateAsync(proxyAccountIndex);
    }
}