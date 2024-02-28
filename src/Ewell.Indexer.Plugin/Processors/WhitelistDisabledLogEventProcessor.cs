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

public class WhitelistDisabledLogEventProcessor : WhitelistProcessorBase<WhitelistDisabled>
{
    public WhitelistDisabledLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistDisabled, LogEventInfo>> logger, 
        IObjectMapper objectMapper, IOptionsSnapshot<ContractInfoOptions> contractInfoOptions, 
        IJsonSerializer jsonSerializer, 
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository) : 
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, whitelistRepository)
    {
    }
    
    protected override async Task HandleEventAsync(WhitelistDisabled eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[WhitelistDisabled] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var whitelist = await WhitelistRepository.GetFromBlockStateSetAsync(whitelistId, chainId);
            if (whitelist != null)
            {
                whitelist.IsAvailable = eventValue.IsAvailable;
            }
            else
            {
                whitelist = ObjectMapper.Map<WhitelistDisabled, WhitelistIndex>(eventValue);
                whitelist.Id = whitelistId;
            }
            ObjectMapper.Map(context, whitelist);
            await WhitelistRepository.AddOrUpdateAsync(whitelist);
            Logger.LogInformation("[WhitelistDisabled] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[WhitelistDisabled] Exception: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
            throw;
        }
    }
}