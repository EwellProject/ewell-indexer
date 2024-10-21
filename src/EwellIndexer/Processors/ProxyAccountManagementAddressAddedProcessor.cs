using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.ProxyAccountContract;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ProxyAccountManagementAddressAddedProcessor : ProxyAccountProcessorBase<ProxyAccountManagementAddressAdded>
{
    public override async Task ProcessAsync(ProxyAccountManagementAddressAdded logEvent, LogEventContext context)
    {
        Logger.LogInformation(
            "[ProxyAccountManagementAddressAdded] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(logEvent.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = await GetEntityAsync<ProxyAccountIndex>(proxyAccountIndexId);
        if (proxyAccountIndex == null)
        {
            Logger.LogInformation(
                "[ProxyAccountManagementAddressAdded] proxyAccountIndexId {proxyAccountIndexId} not exist.",
                proxyAccountIndexId);
            return;
        }

        proxyAccountIndex.ManagersSet.Add(logEvent.ManagementAddress.Address.ToBase58());
        await SaveEntityAsync(proxyAccountIndex);
    }
}