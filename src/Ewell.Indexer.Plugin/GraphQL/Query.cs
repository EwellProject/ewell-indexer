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
        [FromServices] IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> repository,
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

        var result = await repository.GetListAsync(Filter, skip: input.SkipCount,
            limit: input.MaxResultCount, sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        var projectList = objectMapper.Map<List<CrowdfundingProjectIndex>, List<CrowdfundingProjectDto>>(result.Item2);
        return new ProjectListPageResultDto
        {
            TotalCount = result.Item1,
            Data = projectList,
        };
    }
    
    
    [Name("whitelist")]
    public static async Task<WhitelistPageResultDto> WhiteListAsync(
        [FromServices] IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetWhiteListDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<WhitelistIndex>, QueryContainer>>
        {
            q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)),
            q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight))
        };
        QueryContainer Filter(QueryContainerDescriptor<WhitelistIndex> f) => f.Bool(b => b.Must(mustQuery));
        IPromise<IList<ISort>> Sort(SortDescriptor<WhitelistIndex> s) => s
            .Ascending(a => a.BlockHeight)
            .Ascending(a => a.LastModifyTime);
        
        var (totalCount, list) = await repository.GetSortListAsync(Filter, sortFunc: Sort,
            limit: input.MaxResultCount, skip: input.SkipCount);
        var dataList = objectMapper.Map<List<WhitelistIndex>, List<WhitelistDto>>(list);
        
        return new WhitelistPageResultDto(totalCount, dataList);
    }
    
    [Name("whitelistByWhitelistIdAsync")]
    public static async Task<WhitelistDto> WhitelistByWhitelistIdAsync(
        [FromServices] IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetWhiteListDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<WhitelistIndex>, QueryContainer>>
        {
            q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)),
            q => q.Term(i => i.Field(f => f.Id).Value(input.WhitelistId))
        };
        QueryContainer Filter(QueryContainerDescriptor<WhitelistIndex> f) => f.Bool(b => b.Must(mustQuery));
        
        var whitelist = await repository.GetAsync(Filter);
        return whitelist == null ? new WhitelistDto{Id = input.WhitelistId, IsAvailable = true} : objectMapper.Map<WhitelistIndex, WhitelistDto>(whitelist);
    }

    [Name("project")]
    public static async Task<ProjectPageResultDto> ProjectAsync(
        [FromServices] IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetProjectDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<ProjectIndex>, QueryContainer>>
        {
            q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)),
            q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight))
        };
        QueryContainer Filter(QueryContainerDescriptor<ProjectIndex> f) => f.Bool(b => b.Must(mustQuery));
        IPromise<IList<ISort>> Sort(SortDescriptor<ProjectIndex> s) => s
            .Ascending(a => a.BlockHeight)
            .Ascending(a => a.LastModifyTime);
        
        var (totalCount, list) = await repository.GetSortListAsync(Filter, sortFunc: Sort,
            limit: input.MaxResultCount, skip: input.SkipCount);
        var dataList = objectMapper.Map<List<ProjectIndex>, List<ProjectDto>>(list);
        
        return new ProjectPageResultDto(totalCount, dataList);
    }
    
    [Name("projectByWhitelistId")]
    public static async Task<ProjectPageResultDto> ProjectByWhitelistIdAsync(
        [FromServices] IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetProjectDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<ProjectIndex>, QueryContainer>>
        {
            q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)),
            q => q.Term(i => i.Field(f => f.WhitelistId).Value(input.WhitelistId))
        };
        QueryContainer Filter(QueryContainerDescriptor<ProjectIndex> f) => f.Bool(b => b.Must(mustQuery));
        
        var (totalCount, list) = await repository.GetListAsync(Filter, limit: input.MaxResultCount, skip: input.SkipCount);
        var dataList = objectMapper.Map<List<ProjectIndex>, List<ProjectDto>>(list);
        
        return new ProjectPageResultDto(totalCount, dataList);
    }
    
    [Name("claimedDamage")]
    public static async Task<DamageClaimedPageResultDto> ClaimedDamageAsync(
        [FromServices] IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetDamageClaimedDto input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<LiquidatedDamageClaimedIndex>, QueryContainer>>
        {
            q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)),
            q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.StartBlockHeight))
        };
        QueryContainer Filter(QueryContainerDescriptor<LiquidatedDamageClaimedIndex> f) => f.Bool(b => b.Must(mustQuery));
        IPromise<IList<ISort>> Sort(SortDescriptor<LiquidatedDamageClaimedIndex> s) => s
            .Ascending(a => a.BlockHeight)
            .Ascending(a => a.LastModifyTime);
        
        var (totalCount, list) = await repository.GetSortListAsync(Filter, sortFunc: Sort,
            limit: input.MaxResultCount, skip: input.SkipCount);
        var dataList = objectMapper.Map<List<LiquidatedDamageClaimedIndex>, List<DamageClaimedDto>>(list);
        
        return new DamageClaimedPageResultDto(totalCount, dataList);
    }
}