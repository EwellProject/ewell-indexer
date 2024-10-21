using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class AdditionalInfoUpdatedProcessor : ProjectProcessorBase<AdditionalInfoUpdated>
{
    public override async Task ProcessAsync(AdditionalInfoUpdated logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[AdditionalInfoUpdated] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[AdditionalInfoUpdated] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        crowdfundingProject.AdditionalInfo = JsonConvert.SerializeObject(logEvent.AdditionalInfo.Data);
        await SaveEntityAsync(crowdfundingProject);
        Logger.LogInformation("[AdditionalInfoUpdated] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}