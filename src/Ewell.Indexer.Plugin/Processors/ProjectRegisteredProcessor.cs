using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ProjectRegisteredProcessor : AElfLogEventProcessorBase<ProjectRegistered, LogEventInfo>
{
    private readonly ILogger<AElfLogEventProcessorBase<ProjectRegistered, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    private readonly ContractInfoOptions _contractInfoOptions;

    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    public ProjectRegisteredProcessor(ILogger<AElfLogEventProcessorBase<ProjectRegistered, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger)
    {
        _logger = logger;
        _objectMapper = objectMapper;
        _contractInfoOptions = contractInfoOptions.Value;
        _crowdfundingProjectRepository = crowdfundingProjectRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected override async Task HandleEventAsync(ProjectRegistered eventValue, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = eventValue.ProjectId.ToHex();
        _logger.LogInformation("[ProjectRegistered] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject =
            await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject != null)
        {
            _logger.LogInformation(
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
        var projectIndex = _objectMapper.Map<ProjectRegistered, CrowdfundingProjectIndex>(eventValue);
        _objectMapper.Map(context, projectIndex);
        projectIndex.Id = projectId;
        projectIndex.ListMarketInfo = marketInformation;
        projectIndex.AdditionalInfo = additionalInformation;
        projectIndex.IsCanceled = false;
        projectIndex.ToRaiseToken = new TokenBasicInfo { ChainId = chainId, Symbol = eventValue.AcceptedCurrency };
        projectIndex.CrowdFundingIssueToken = new TokenBasicInfo { ChainId = chainId, Symbol = eventValue.ProjectCurrency };
        await _crowdfundingProjectRepository.AddOrUpdateAsync(projectIndex);
        _logger.LogInformation("[ProjectRegistered] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}