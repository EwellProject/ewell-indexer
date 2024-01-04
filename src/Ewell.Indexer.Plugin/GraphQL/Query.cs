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
}