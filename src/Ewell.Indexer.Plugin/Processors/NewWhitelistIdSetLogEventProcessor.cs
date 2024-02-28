using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class NewWhitelistIdSetLogEventProcessor : ProjectProcessorBase<NewWhitelistIdSet>
{
    protected readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> WhitelistRepository;
    
    public NewWhitelistIdSetLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
        WhitelistRepository = whitelistRepository;
    }

    protected override async Task HandleEventAsync(NewWhitelistIdSet eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var chainId = context.ChainId;
        var whitelistId = eventValue.WhitelistId.ToHex();
        Logger.LogInformation("[NewWhitelistIdSet] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            projectId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var crowdfundingProject = await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
            if (crowdfundingProject == null)
            {
                Logger.LogInformation("[NewWhitelistIdSet] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId, chainId);
                return;
            }
            crowdfundingProject.WhitelistId = eventValue.WhitelistId.ToHex();
            ObjectMapper.Map(context, crowdfundingProject);

            Logger.LogInformation("[NewWhitelistIdSet] SAVE: Id={Id}", projectId);
            await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
            Logger.LogInformation("[NewWhitelistIdSet] FINISH: Id={Id}", projectId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[NewWhitelistIdSet] Exception Id={Id}", projectId);
            throw;
        }
    }
}