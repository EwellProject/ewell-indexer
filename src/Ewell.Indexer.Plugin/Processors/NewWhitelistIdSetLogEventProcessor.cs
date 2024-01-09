using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class NewWhitelistIdSetLogEventProcessor : ProjectProcessorBase<NewWhitelistIdSet>
{
    public NewWhitelistIdSetLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
    }

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoOptions.ContractInfos[chainId].WhitelistContractAddress;
    }

    protected override async Task HandleEventAsync(NewWhitelistIdSet eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var chainId = context.ChainId;
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