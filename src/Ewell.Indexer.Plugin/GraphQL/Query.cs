using AElfIndexer.Client;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.Grain.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using GraphQL;
using Nest;
using Orleans;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.GraphQL;

public class Query
{
    [Name("syncState")]
    public static async Task<SyncStateDto> SyncStateAsync(
        [FromServices] IClusterClient clusterClient, 
        [FromServices] IAElfIndexerClientInfoProvider clientInfoProvider,
        GetSyncStateDto input)
    {
        var version = clientInfoProvider.GetVersion();
        var clientId = clientInfoProvider.GetClientId();
        var blockStateSetInfoGrain =
            clusterClient.GetGrain<IBlockStateSetInfoGrain>(
                GrainIdHelper.GenerateGrainId("BlockStateSetInfo", clientId, input.ChainId, version));
        var confirmedHeight = await blockStateSetInfoGrain.GetConfirmedBlockHeight(input.FilterType);
        return new SyncStateDto
        {
            ConfirmedBlockHeight = confirmedHeight
        };
    }
    
    [Name("getProjectList")]
    public static async Task<ProjectListPageResultDto> GetProjectListAsync(
        [FromServices] IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> projectRepository,
        [FromServices] IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> whitelistRepository,
        [FromServices] IObjectMapper objectMapper,
        GetProjectListInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<CrowdfundingProjectIndex>, QueryContainer>>();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            mustQuery.Add(q => q.Term(i
                => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        if (input.StartBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight)));
        }
        if (input.EndBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight)));
        }
        QueryContainer Filter(QueryContainerDescriptor<CrowdfundingProjectIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await projectRepository.GetListAsync(Filter, skip: input.SkipCount,
            limit: input.MaxResultCount, sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        var projectList = objectMapper.Map<List<CrowdfundingProjectIndex>, List<CrowdfundingProjectDto>>(result.Item2);
        
        //whitelist
        var whitelistIds = projectList.Select(x => x.WhitelistId).ToList();
        var whitelistMustQuery = new List<Func<QueryContainerDescriptor<WhitelistIndex>, QueryContainer>>();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            whitelistMustQuery.Add(q => q.Term(i
                => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        if (!whitelistIds.IsNullOrEmpty())
        {
            whitelistMustQuery.Add(q => q.Terms(i
                => i.Field(f => f.Id).Terms(whitelistIds)));
        }
        QueryContainer WhitelistFilter(QueryContainerDescriptor<WhitelistIndex> f) =>
            f.Bool(b => b.Must(whitelistMustQuery));
        var whitelistList = (await whitelistRepository.GetListAsync(WhitelistFilter)).Item2;
        var whitelistMap = whitelistList.ToDictionary(whitelist => whitelist.Id);
        foreach (var project in projectList.Where(project => whitelistMap.ContainsKey(project.WhitelistId)))
        {
            project.IsEnableWhitelist = whitelistMap.GetOrDefault(project.WhitelistId).IsAvailable;
        }

        return new ProjectListPageResultDto
        {
            TotalCount = result.Item1,
            Data = projectList,
        };
    }
    
    [Name("getUserRecordList")]
    public static async Task<UserRecordResultDto> GetUserRecordListAsync(
        [FromServices] IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetUserRecordInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<UserRecordIndex>, QueryContainer>>();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            mustQuery.Add(q => q.Term(i
                => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        if (input.StartBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight)));
        }
        if (input.EndBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight)));
        }
        QueryContainer Filter(QueryContainerDescriptor<UserRecordIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, skip: input.SkipCount,
            limit: input.MaxResultCount, sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        var projectList = objectMapper.Map<List<UserRecordIndex>, List<UserRecordDto>>(result.Item2);
        return new UserRecordResultDto
        {
            TotalCount = result.Item1, 
            Data = projectList
        };
    }
    
    [Name("getSyncUserProjectInfos")]
    public static async Task<List<UserProjectInfoSyncDto>> GetSyncUserProjectInfosAsync(
        [FromServices] IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetChainBlockHeightDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<UserProjectInfoIndex>, QueryContainer>>();
        mustQuery.Add(q => q.Term(i
            => i.Field(f => f.ChainId).Value(input.ChainId)));

        if (input.StartBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight)));
        }

        if (input.EndBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight)));
        }

        QueryContainer Filter(QueryContainerDescriptor<UserProjectInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, skip: input.SkipCount, 
            sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        return objectMapper.Map<List<UserProjectInfoIndex>, List<UserProjectInfoSyncDto>>(result.Item2);
    }
    
    [Name("getWhitelistList")]
    public static async Task<WhitelistResultDto> GetWhitelistListAsync(
        [FromServices] IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetWhitelistInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<WhitelistIndex>, QueryContainer>>();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            mustQuery.Add(q => q.Term(i
                => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        if (input.StartBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight)));
        }
        if (input.EndBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight)));
        }
        QueryContainer Filter(QueryContainerDescriptor<WhitelistIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, skip: input.SkipCount,
            limit: input.MaxResultCount, sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        var list = objectMapper.Map<List<WhitelistIndex>, List<WhitelistDto>>(result.Item2);
        return new WhitelistResultDto
        {
            TotalCount = result.Item1,
            Data = list
        };
    }
    
    [Name("getUserTokenInfos")]
    public static async Task<List<GetUserTokensDto>> GetUserTokenInfosAsync(
        [FromServices] IAElfIndexerClientEntityRepository<TokenInfoIndex, LogEventInfo> tokenIndexRepository,
        [FromServices] IAElfIndexerClientEntityRepository<UserBalanceIndex, LogEventInfo> userBalanceIndexRepository,
        [FromServices] IObjectMapper objectMapper,
        GetUserTokensInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<UserBalanceIndex>, QueryContainer>>();
      
        mustQuery.Add(q => q.Term(i
            => i.Field(f => f.ChainId).Value(input.ChainId)));
        
        mustQuery.Add(q => q.Term(i
            => i.Field(f => f.Address).Value(input.Address)));

        QueryContainer Filter(QueryContainerDescriptor<UserBalanceIndex> f) =>
            f.Bool(b => b.Must(mustQuery));
        
        var result = await userBalanceIndexRepository.GetListAsync(Filter);

        if (result.Item2.IsNullOrEmpty())
        {
            return new List<GetUserTokensDto>();
        }

        var tokenIds = result.Item2.Select(item => IdGenerateHelper.GetTokenInfoId(item.ChainId, item.Symbol))
            .ToList();
        
        var tokenInfos = await GetTokenInfosAsync(tokenIndexRepository, tokenIds);

        return result.Item2.Select(item =>
            {
                var id = IdGenerateHelper.GetTokenInfoId(item.ChainId, item.Symbol);
                if (tokenInfos.TryGetValue(id, out var tokenInfo))
                {
                    var userTokensDto = objectMapper.Map<TokenInfoIndex, GetUserTokensDto>(tokenInfo);
                    userTokensDto.Balance = item.Amount;
                    userTokensDto.ImageUrl =  tokenInfo.ExternalInfoDictionary
                        .FirstOrDefault(o => o.Key == "__nft_image_url")?.Value;
                    return userTokensDto;
                }
                return null;
            }
        ).ToList();
    }

    private static async Task<Dictionary<string, TokenInfoIndex>> GetTokenInfosAsync(
        [FromServices] IAElfIndexerClientEntityRepository<TokenInfoIndex, LogEventInfo> tokenIndexRepository,
        List<string> tokenIds)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<TokenInfoIndex>, QueryContainer>>();

        mustQuery.Add(q => q.Terms(i
            => i.Field(f => f.Id).Terms(tokenIds)));

        QueryContainer Filter(QueryContainerDescriptor<TokenInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await tokenIndexRepository.GetListAsync(Filter);
        return result.Item2.IsNullOrEmpty()
            ? new Dictionary<string, TokenInfoIndex>()
            : result.Item2.ToDictionary(item => item.Id, item => item);
    }
}