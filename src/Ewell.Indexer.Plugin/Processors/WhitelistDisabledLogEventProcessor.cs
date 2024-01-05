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
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> _crowdfundingProjectRepository;
    private readonly IObjectMapper _objectMapper;


    public WhitelistDisabledLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<WhitelistDisabled, LogEventInfo>> logger,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IObjectMapper objectMapper) : base(logger)
    {
        _logger = logger;
        _crowdfundingProjectRepository = crowdfundingProjectRepository;
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
        _logger.LogInformation("[WhitelistDisabled] START: Id={Id}, Event={Event} , ChainId={ChainId}",
            whitelistId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            const int maxResultCount = 500;
            var skipCount = 0;
            var toUpdate = new List<CrowdfundingProjectIndex>();
            while (true)
            {
                var mustQuery = new List<Func<QueryContainerDescriptor<CrowdfundingProjectIndex>, QueryContainer>>
                {
                    q => q.Term(i 
                        => i.Field(f => f.ChainId).Value(chainId)),
                    q => q.Term(i 
                    => i.Field(f => f.WhitelistId).Value(whitelistId))
                };
                QueryContainer Filter(QueryContainerDescriptor<CrowdfundingProjectIndex> f) =>
                    f.Bool(b => b.Must(mustQuery));
                
                var (_, dataList) = await _crowdfundingProjectRepository.GetListAsync(Filter,null, 
                    o => o.BlockHeight, sortType: SortOrder.Ascending, skipCount, maxResultCount);
                if (dataList.Count < maxResultCount)
                {
                    break;
                }

                skipCount += dataList.Count;
                toUpdate.AddRange(dataList);
            }

            if (!toUpdate.IsNullOrEmpty())
            {
                _logger.LogInformation("[WhitelistDisabled] SAVE: Id={Id}", whitelistId);
                foreach (var crowdfundingProjectIndex in toUpdate)
                {
                    crowdfundingProjectIndex.IsEnableWhitelist = eventValue.IsAvailable;
                    _objectMapper.Map(context, crowdfundingProjectIndex);
                    await _crowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProjectIndex);
                }
                _logger.LogInformation("[WhitelistDisabled] FINISH: Id={Id}", whitelistId);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[WhitelistDisabled] Exception: Id={Id}", whitelistId);
            throw;
        }
    }
}