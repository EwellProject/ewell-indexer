using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ProjectCanceledProcessor : ProjectProcessorBase<ProjectCanceled>
{
    public ProjectCanceledProcessor(
        ILogger<AElfLogEventProcessorBase<ProjectCanceled, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
    }
    
    protected override async Task HandleEventAsync(ProjectCanceled eventValue, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = eventValue.ProjectId.ToHex();
        Logger.LogInformation("[ProjectCanceled] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[ProjectCanceled] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        ObjectMapper.Map(context, crowdfundingProject);
        crowdfundingProject.IsCanceled = true;
        crowdfundingProject.CancelTime = context.BlockTime;
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        Logger.LogInformation("[ProjectCanceled] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}