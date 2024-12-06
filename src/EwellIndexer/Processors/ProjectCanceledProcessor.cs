using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class ProjectCanceledProcessor : ProjectProcessorBase<ProjectCanceled>
{
    public override async Task ProcessAsync(ProjectCanceled logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[ProjectCanceled] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[ProjectCanceled] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        crowdfundingProject.IsCanceled = true;
        crowdfundingProject.CancelTime = context.Block.BlockTime;
        await SaveEntityAsync(crowdfundingProject);
        Logger.LogInformation("[ProjectCanceled] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}