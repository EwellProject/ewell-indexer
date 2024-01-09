using AElf.Contracts.Whitelist;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Orleans.Runtime;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class WhitelistReenableLogEventProcessor : AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>
{
    private readonly ContractInfoOptions _contractInfoOptions;
    private readonly ILogger<AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>> _logger;
    private readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistRepository;
    private readonly IObjectMapper _objectMapper;


    public WhitelistReenableLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistReenable, LogEventInfo>> logger,
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

    protected override async Task HandleEventAsync(WhitelistReenable eventValue, LogEventContext context)
    {
        var whitelistId = eventValue.WhitelistId.ToHex();
        var chainId = context.ChainId;
        _logger.LogInformation("[WhitelistReenable] START: Id={Id}, Event={Event}",
            whitelistId, JsonConvert.SerializeObject(eventValue));
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
                whitelist = _objectMapper.Map<WhitelistReenable, WhitelistIndex>(eventValue);
                whitelist.Id = whitelistId;
            }
            _objectMapper.Map(context, whitelist);
            await _whitelistRepository.AddOrUpdateAsync(whitelist);
            _logger.LogInformation("[WhitelistReenable] FINISH: Id={Id}, ChainId={ChainId}",
                whitelistId, chainId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[NewWhitelistIdSet] Exception Id={Id}", whitelistId);
            throw;
        }
    }
}