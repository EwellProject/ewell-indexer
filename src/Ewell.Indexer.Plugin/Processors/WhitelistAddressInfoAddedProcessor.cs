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

public class WhitelistAddressInfoAddedProcessor : WhitelistProcessorBase<WhitelistAddressInfoAdded>
{
    public WhitelistAddressInfoAddedProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistAddressInfoAdded, LogEventInfo>> logger, 
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions, 
        IJsonSerializer jsonSerializer, 
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository) : 
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, whitelistRepository)
    {
    }
    
    protected override async Task HandleEventAsync(WhitelistAddressInfoAdded eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[AddAddressInfoListToWhitelist] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var whitelist = await WhitelistRepository.GetFromBlockStateSetAsync(whitelistId, chainId);
            if (whitelist == null)
            {
                Logger.LogInformation("[AddAddressInfoListToWhitelist] whitelistIndex not exist: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
                return;
            }

            var toAdd = new List<WhitelistAddressTime>();
            foreach (var extraInfo in eventValue.ExtraInfoIdList.Value)
            {
                toAdd.AddRange(extraInfo.AddressList.Value.Select(address => new WhitelistAddressTime
                {
                    CreateTime = context.BlockTime, 
                    Address = address.ToBase58()
                }));
            }
            var existed = JsonConvert.DeserializeObject<List<WhitelistAddressTime>>(whitelist.AddressTimeInfo);
            existed.AddRange(toAdd);
            whitelist.AddressTimeInfo = JsonConvert.SerializeObject(existed);
            ObjectMapper.Map(context, whitelist);
            await WhitelistRepository.AddOrUpdateAsync(whitelist);
            Logger.LogInformation("[AddAddressInfoListToWhitelist] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[AddAddressInfoListToWhitelist] Exception: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
            throw;
        }
    }
}