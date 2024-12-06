using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.ProxyAccountContract;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ProxyAccountCreatedProcessor : ProxyAccountProcessorBase<ProxyAccountCreated>
{
    public override async Task ProcessAsync(ProxyAccountCreated logEvent, LogEventContext context)
    {
        Logger.LogInformation("[ProxyAccountCreated] handle chainId {chainId} proxyAccountAddress {proxyAccountAddress}",
            context.ChainId, logEvent?.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(logEvent?.ProxyAccountAddress?.ToBase58());
        var proxyAccountIndex = ObjectMapper.Map<ProxyAccountCreated, ProxyAccountIndex>(logEvent);
        proxyAccountIndex.Id = proxyAccountIndexId;
        proxyAccountIndex.CreateTime = context.Block.BlockTime;
        await SaveEntityAsync(proxyAccountIndex);
    }
}