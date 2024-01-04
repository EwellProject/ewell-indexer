using AElf.Contracts.Whitelist;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class WhitelistReenableLogEventProcessor : AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>
{
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>> _logger;
    private readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistIndexRepository;
    private readonly IObjectMapper _objectMapper;


    public WhitelistReenableLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistIndexRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _logger = logger;
        _whitelistIndexRepository = whitelistIndexRepository;
        _objectMapper = objectMapper;
        _contractInfoOptions = contractInfoOptions.Value;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].WhitelistContractAddress;
    }

    protected override async Task HandleEventAsync(WhitelistReenable eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        _logger.LogInformation("[WhitelistReenable] START: Id={Id}, Event={Event}", 
            whitelistId, JsonConvert.SerializeObject(eventValue));
        try
        {
            var whitelist = await _whitelistIndexRepository.GetFromBlockStateSetAsync(whitelistId, context.ChainId);
            if (whitelist != null)
            {
                whitelist.IsAvailable = eventValue.IsAvailable;
            }
            else
            {
                whitelist = _objectMapper.Map<WhitelistReenable, WhitelistIndex>(eventValue);
                whitelist.Id = whitelistId;
            }
            whitelist.LastModifyTime = DateTimeHelper.GetTimeStampInMilliseconds();
            _objectMapper.Map(context, whitelist);
        
            _logger.LogInformation("[WhitelistReenable] SAVE: Id={Id}", whitelistId);
            await _whitelistIndexRepository.AddOrUpdateAsync(whitelist);
            _logger.LogInformation("[WhitelistReenable] FINISH: Id={Id}", whitelistId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[WhitelistReenable] FAILED: Id={Id}", whitelistId);
            throw;
        }
    }
}