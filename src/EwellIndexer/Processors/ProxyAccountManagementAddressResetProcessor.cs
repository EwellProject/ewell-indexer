using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.ProxyAccountContract;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ProxyAccountManagementAddressResetProcessor : ProxyAccountProcessorBase<ProxyAccountManagementAddressReset>
{
    public override async Task ProcessAsync(ProxyAccountManagementAddressReset logEvent, LogEventContext context)
    {
        Logger.LogInformation(
            "[ProxyAccountManagementAddressReset] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = ObjectMapper.Map<ProxyAccountManagementAddressReset, ProxyAccountIndex>(logEvent);
        proxyAccountIndex.Id = proxyAccountIndexId;
        await SaveEntityAsync(proxyAccountIndex);
    }
}