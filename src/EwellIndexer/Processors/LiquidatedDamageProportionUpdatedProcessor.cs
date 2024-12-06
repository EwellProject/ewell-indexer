using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class LiquidatedDamageProportionUpdatedProcessor : ProjectProcessorBase<LiquidatedDamageProportionUpdated>
{
    public override async Task ProcessAsync(LiquidatedDamageProportionUpdated logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[LiquidatedDamageProportionUpdated] start projectId:{projectId} chainId:{chainId} ", projectId,
            chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[LiquidatedDamageProportionUpdated] crowd funding project with id {id} chainId {chainId} has not existed.",
                projectId,
                chainId);
            return;
        }
        crowdfundingProject.LiquidatedDamageProportion = logEvent.LiquidatedDamageProportion;
        await SaveEntityAsync(crowdfundingProject);
        Logger.LogInformation("[LiquidatedDamageProportionUpdated] end projectId:{projectId} chainId:{chainId} ", projectId,
            chainId);
    }
}