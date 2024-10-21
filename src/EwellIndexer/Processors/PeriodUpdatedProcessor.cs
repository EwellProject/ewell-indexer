using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class PeriodUpdatedProcessor : ProjectProcessorBase<PeriodUpdated>
{ 
    public override async Task ProcessAsync(PeriodUpdated logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[PeriodUpdated] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[PeriodUpdated] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        crowdfundingProject.CurrentPeriod = logEvent.NewPeriod;
        await SaveEntityAsync(crowdfundingProject);
        Logger.LogInformation("[PeriodUpdated] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}