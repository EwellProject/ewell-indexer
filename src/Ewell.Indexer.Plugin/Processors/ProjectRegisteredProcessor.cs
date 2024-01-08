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

public class ProjectRegisteredProcessor : ProjectProcessorBase<ProjectRegistered>
{
    public ProjectRegisteredProcessor(
        ILogger<AElfLogEventProcessorBase<ProjectRegistered, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
    }

    protected override async Task HandleEventAsync(ProjectRegistered eventValue, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = eventValue.ProjectId.ToHex();
        Logger.LogInformation("[ProjectRegistered] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject != null)
        {
            Logger.LogInformation(
                "[ProjectRegistered] crowd funding  project with id {id} chainId {chainId} has existed.", projectId,
                chainId);
            return;
        }

        var additionalInformation = eventValue.AdditionalInfo != null
            ? JsonConvert.SerializeObject(eventValue.AdditionalInfo.Data)
            : string.Empty;
        var marketInformation = eventValue.ListMarketInfo != null
            ? JsonConvert.SerializeObject(eventValue.ListMarketInfo.Data.Select(l => new
            {
                Market = l.Market.ToBase58(), l.Weight
            }).ToList())
            : string.Empty;
        var projectIndex = ObjectMapper.Map<ProjectRegistered, CrowdfundingProjectIndex>(eventValue);
        ObjectMapper.Map(context, projectIndex);
        projectIndex.Id = projectId;
        projectIndex.ListMarketInfo = marketInformation;
        projectIndex.AdditionalInfo = additionalInformation;
        projectIndex.IsCanceled = false;
        projectIndex.ToRaiseToken = new TokenBasicInfo { ChainId = chainId, Symbol = eventValue.AcceptedCurrency };
        projectIndex.CrowdFundingIssueToken = new TokenBasicInfo { ChainId = chainId, Symbol = eventValue.ProjectCurrency };
        projectIndex.CreateTime = context.BlockTime;
        await CrowdfundingProjectRepository.AddOrUpdateAsync(projectIndex);
        Logger.LogInformation("[ProjectRegistered] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}