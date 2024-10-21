using System;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class NewWhitelistIdSetLogEventProcessor : ProjectProcessorBase<NewWhitelistIdSet>
{
    public override async Task ProcessAsync(NewWhitelistIdSet logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var chainId = context.ChainId;
        var whitelistId = logEvent.WhitelistId?.ToHex() ?? string.Empty;
        Logger.LogInformation("[NewWhitelistIdSet] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            projectId, JsonConvert.SerializeObject(logEvent), chainId);
        try
        {
            var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);;
            if (crowdfundingProject == null)
            {
                Logger.LogInformation("[NewWhitelistIdSet] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId, chainId);
                return;
            }
            crowdfundingProject.WhitelistId = whitelistId;
            Logger.LogInformation("[NewWhitelistIdSet] SAVE: Id={Id}", projectId);
            await SaveEntityAsync(crowdfundingProject);
            Logger.LogInformation("[NewWhitelistIdSet] FINISH: Id={Id}", projectId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[NewWhitelistIdSet] Exception Id={Id}", projectId);
            throw;
        }
    }
}