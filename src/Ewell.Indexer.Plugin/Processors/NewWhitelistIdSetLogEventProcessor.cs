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
    private readonly IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> _projectIndexRepository;
    private readonly ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    
    public NewWhitelistIdSetLogEventProcessor(ILogger<AElfLogEventProcessorBase<NewWhitelistIdSet, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> projectIndexRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _contractInfoOptions = contractInfoOptions.Value;
        _logger = logger;
        _projectIndexRepository = projectIndexRepository;
        _objectMapper = objectMapper;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].WhitelistContractAddress;
    }

    protected override async Task HandleEventAsync(NewWhitelistIdSet eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        _logger.LogInformation("[NewWhitelistIdSet] START: Id={Id}, Event={Event}",
            projectId, JsonConvert.SerializeObject(eventValue));
        try
        {
            var project = await _projectIndexRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
            if (project != null)
            {
                project.WhitelistId = eventValue.WhitelistId.ToHex();
            }
            else
            {
                project = _objectMapper.Map<NewWhitelistIdSet, ProjectIndex>(eventValue);
                project.WhitelistId = eventValue.WhitelistId.ToHex();
                project.Id = projectId;
            }
            project.LastModifyTime = DateTimeHelper.GetTimeStampInMilliseconds();
            _objectMapper.Map(context, project);

            _logger.LogInformation("[NewWhitelistIdSet] SAVE: Id={Id}", projectId);
            await _projectIndexRepository.AddOrUpdateAsync(project);
            _logger.LogInformation("[NewWhitelistIdSet] FINISH: Id={Id}", projectId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[NewWhitelistIdSet] Exception Id={Id}", projectId);
            throw;
        }
    }
}