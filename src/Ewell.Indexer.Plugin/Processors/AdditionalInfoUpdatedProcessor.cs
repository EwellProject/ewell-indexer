using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class AdditionalInfoUpdatedProcessor : ProjectProcessorBase<AdditionalInfoUpdated>
{
    public AdditionalInfoUpdatedProcessor(
        ILogger<AElfLogEventProcessorBase<AdditionalInfoUpdated, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
    }
    
    protected override async Task HandleEventAsync(AdditionalInfoUpdated eventValue, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = eventValue.ProjectId.ToHex();
        Logger.LogInformation("[AdditionalInfoUpdated] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[AdditionalInfoUpdated] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }

        crowdfundingProject.AdditionalInfo = JsonSerializer.Serialize(eventValue.AdditionalInfo.Data);
        ObjectMapper.Map(context, crowdfundingProject);
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        Logger.LogInformation("[AdditionalInfoUpdated] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}