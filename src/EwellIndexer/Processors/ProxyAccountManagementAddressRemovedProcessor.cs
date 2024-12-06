using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.ProxyAccountContract;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ProxyAccountManagementAddressRemovedProcessor : ProxyAccountProcessorBase<ProxyAccountManagementAddressRemoved>
{
    public override async Task ProcessAsync(ProxyAccountManagementAddressRemoved logEvent, LogEventContext context)
    {
        Logger.LogInformation(
            "[ProxyAccountManagementAddressRemoved] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = await GetEntityAsync<ProxyAccountIndex>(proxyAccountIndexId);
        if (proxyAccountIndex == null)
        {
            Logger.LogInformation(
                "[ProxyAccountManagementAddressRemoved] proxyAccountIndexId {proxyAccountIndexId} not exist.",
                proxyAccountIndexId);
            return;
        }

        proxyAccountIndex.ManagersSet.Remove(logEvent.ManagementAddress.Address.ToBase58());
        await SaveEntityAsync(proxyAccountIndex);
    }
}