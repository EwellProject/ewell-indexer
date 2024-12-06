using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;

namespace EwellIndexer.Processors;

public class WithdrawnProcessor : ProjectProcessorBase<Withdrawn>
{
    public override async Task ProcessAsync(Withdrawn logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[Withdrawn] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[Withdrawn] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        ObjectMapper.Map(context, crowdfundingProject);
        crowdfundingProject.IsWithdraw = true;
        crowdfundingProject.WithdrawTime = context.Block.BlockTime;
        await SaveEntityAsync(crowdfundingProject);
        Logger.LogInformation("[Withdrawn] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}