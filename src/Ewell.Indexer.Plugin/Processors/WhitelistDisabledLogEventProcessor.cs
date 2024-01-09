using System.Runtime.InteropServices.JavaScript;
using AElf.Contracts.Whitelist;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class WhitelistDisabledLogEventProcessor : AElfLogEventProcessorBase<WhitelistDisabled, LogEventInfo>
{
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<AElfLogEventProcessorBase<WhitelistDisabled, LogEventInfo>> _logger;
    private readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistRepository;
    private readonly IObjectMapper _objectMapper;
    
    public WhitelistDisabledLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistDisabled, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _logger = logger;
        _whitelistRepository = whitelistRepository;
        _objectMapper = objectMapper;
        _contractInfoOptions = contractInfoOptions.Value;
    }

    public override string GetContractAddress(string chainId)
    {
        return _contractInfoOptions.ContractInfos[chainId].WhitelistContractAddress;
    }

    protected override async Task HandleEventAsync(WhitelistDisabled eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        var chainId = context.ChainId;
        _logger.LogInformation("[WhitelistDisabled] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var whitelist = await _whitelistRepository.GetFromBlockStateSetAsync(whitelistId, chainId);
            if (whitelist != null)
            {
                whitelist.IsAvailable = eventValue.IsAvailable;
                await _whitelistRepository.AddOrUpdateAsync(whitelist);
            }
            else
            {
                whitelist = _objectMapper.Map<WhitelistDisabled, WhitelistIndex>(eventValue);
                whitelist.Id = whitelistId;
            }
            _objectMapper.Map(context, whitelist);
            await _whitelistRepository.AddOrUpdateAsync(whitelist);
            _logger.LogInformation("[WhitelistDisabled] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[WhitelistDisabled] Exception: Id={Id}, ChainId={ChainId}", whitelistId, chainId);
            throw;
        }
    }
}