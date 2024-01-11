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
        return new UserRecordResultDto(result.Item1, projectList);
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
        var projectList = objectMapper.Map<List<WhitelistIndex>, List<WhitelistDto>>(result.Item2);
        return new WhitelistResultDto(result.Item1, projectList);
    }
}