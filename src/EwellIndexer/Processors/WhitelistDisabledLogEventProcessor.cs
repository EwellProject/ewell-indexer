using System;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.Whitelist;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class WhitelistDisabledLogEventProcessor : WhitelistProcessorBase<WhitelistDisabled>
{
    public override async Task ProcessAsync(WhitelistDisabled logEvent, LogEventContext context)
    {
        var whitelistId = logEvent.WhitelistId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[WhitelistDisabled] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(logEvent), chainId);
        try
        {
            var whitelist = await GetEntityAsync<WhitelistIndex>(whitelistId);
            if (whitelist != null)
            {
                whitelist.IsAvailable = logEvent.IsAvailable;
            }
            else
            {
                whitelist = ObjectMapper.Map<WhitelistDisabled, WhitelistIndex>(logEvent);
                whitelist.Id = whitelistId;
            }
            await SaveEntityAsync(whitelist);
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