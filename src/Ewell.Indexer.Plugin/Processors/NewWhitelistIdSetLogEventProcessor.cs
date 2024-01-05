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

public class NewWhitelistIdSetLogEventProcessor : AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>
{
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> _crowdfundingProjectRepository;
    private readonly ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    
    public NewWhitelistIdSetLogEventProcessor(ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _contractInfoOptions = contractInfoOptions.Value;
        _logger = logger;
        _crowdfundingProjectRepository = crowdfundingProjectRepository;
        _objectMapper = objectMapper;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].WhitelistContractAddress;
    }

    protected override async Task HandleEventAsync(NewWhitelistIdSet eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var chainId = context.ChainId;
        _logger.LogInformation("[NewWhitelistIdSet] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            projectId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var crowdfundingProject = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
            if (crowdfundingProject == null)
            {
                _logger.LogInformation("[NewWhitelistIdSet] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId, chainId);
                return;
            }
            crowdfundingProject.WhitelistId = eventValue.WhitelistId.ToHex();
            _objectMapper.Map(context, crowdfundingProject);

            _logger.LogInformation("[NewWhitelistIdSet] SAVE: Id={Id}", projectId);
            await _crowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
            _logger.LogInformation("[NewWhitelistIdSet] FINISH: Id={Id}", projectId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[NewWhitelistIdSet] Exception Id={Id}", projectId);
            throw;
        }
    }
}