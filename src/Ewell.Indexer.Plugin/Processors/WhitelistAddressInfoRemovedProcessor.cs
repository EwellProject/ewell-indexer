using AElf.Contracts.Whitelist;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class WhitelistAddressInfoRemovedProcessor : WhitelistProcessorBase<WhitelistAddressInfoRemoved>
{
    public WhitelistAddressInfoRemovedProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistAddressInfoRemoved, LogEventInfo>> logger, 
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions, 
        IJsonSerializer jsonSerializer, 
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository) : 
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, whitelistRepository)
    {
    }
    
    protected override async Task HandleEventAsync(WhitelistAddressInfoRemoved eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[WhitelistAddressInfoRemoved] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var whitelist = await WhitelistRepository.GetFromBlockStateSetAsync(whitelistId, chainId);
            if (whitelist == null)
            {
                Logger.LogInformation("[WhitelistAddressInfoRemoved] whitelistIndex not exist: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
                return;
            }

            var toRemove = new List<String>();
            foreach (var extraInfo in eventValue.ExtraInfoIdList.Value)
            {
                toRemove.AddRange(extraInfo.AddressList.Value.Select(address => address.ToBase58()));
            }
            var existed = JsonConvert.DeserializeObject<List<WhitelistAddressTime>>(whitelist.AddressTimeInfo);
            existed.RemoveAll(x => toRemove.Contains(x.Address));
            whitelist.AddressTimeInfo = JsonConvert.SerializeObject(existed);
            ObjectMapper.Map(context, whitelist);
            await WhitelistRepository.AddOrUpdateAsync(whitelist);
            Logger.LogInformation("[WhitelistAddressInfoRemoved] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[AddAddressInfoListToWhitelist] Exception: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
            throw;
        }
    }
}