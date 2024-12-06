using System;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using AElf.Contracts.Whitelist;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class WhitelistReenableLogEventProcessor : WhitelistProcessorBase<WhitelistReenable>
{

    public override async Task ProcessAsync(WhitelistReenable logEvent, LogEventContext context)
    {
        var whitelistId = logEvent.WhitelistId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[WhitelistReenable] START: Id={Id}, Event={Event}",
            whitelistId, JsonConvert.SerializeObject(logEvent));
        try
        {
            var whitelist = await GetEntityAsync<WhitelistIndex>(whitelistId);
            if (whitelist != null)
            {
                whitelist.IsAvailable = logEvent.IsAvailable;
            }
            else
            {
                whitelist = ObjectMapper.Map<WhitelistReenable, WhitelistIndex>(logEvent);
                whitelist.Id = whitelistId;
            }
            await SaveEntityAsync(whitelist);
            Logger.LogInformation("[WhitelistReenable] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[WhitelistReenable] Exception Id={Id}", whitelistId);
            throw;
        }
    }
}