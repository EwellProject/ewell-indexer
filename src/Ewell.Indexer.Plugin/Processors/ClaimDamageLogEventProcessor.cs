using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ClaimDamageLogEventProcessor : AElfLogEventProcessorBase<LiquidatedDamageClaimed, LogEventInfo>
{
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo> _damageIndexRepository;
    private readonly ILogger<AElfLogEventProcessorBase<LiquidatedDamageClaimed, LogEventInfo>> _logger;
    private readonly IObjectMapper _objectMapper;
    
    public ClaimDamageLogEventProcessor(ILogger<AElfLogEventProcessorBase<LiquidatedDamageClaimed, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo> damageIndexRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _contractInfoOptions = contractInfoOptions.Value;
        _logger = logger;
        _damageIndexRepository = damageIndexRepository;
        _objectMapper = objectMapper;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidatedDamageClaimed eventValue, LogEventContext context)
    {
        var id = IdGenerateHelper.GetId(eventValue.ProjectId.ToHex(), eventValue.User.ToBase58());
        _logger.LogInformation("[LiquidatedDamageClaimed] START: Id={Id}, Event={Event}", 
            id, JsonConvert.SerializeObject(eventValue));
        try
        {
            var liquidatedDamageClaimed = await _damageIndexRepository.GetFromBlockStateSetAsync(id, context.ChainId);
            if (liquidatedDamageClaimed != null) throw new UserFriendlyException("liquidatedDamageClaimed exists");
            
            liquidatedDamageClaimed = _objectMapper.Map<LiquidatedDamageClaimed, LiquidatedDamageClaimedIndex>(eventValue);
            liquidatedDamageClaimed.Id = id;
            liquidatedDamageClaimed.ProjectId = eventValue.ProjectId.ToHex();
            liquidatedDamageClaimed.User = eventValue.User.ToBase58();
            liquidatedDamageClaimed.LastModifyTime = DateTimeHelper.GetTimeStampInMilliseconds();
            _objectMapper.Map(context, liquidatedDamageClaimed);
            
            _logger.LogInformation("[LiquidatedDamageClaimed] SAVE: Id={Id}", id);
            await _damageIndexRepository.AddOrUpdateAsync(liquidatedDamageClaimed);
            _logger.LogInformation("[LiquidatedDamageClaimed] FINISH: Id={Id}", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[LiquidatedDamageClaimed] Exception Id={projectId}", id);
            throw;
        }
    }
}