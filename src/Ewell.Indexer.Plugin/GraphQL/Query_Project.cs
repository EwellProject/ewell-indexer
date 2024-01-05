using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using GraphQL;
using Nest;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.GraphQL;

public partial class Query
{
    
    [Name("getSyncUserProjectInfos")]
    public static async Task<List<UserProjectInfoSyncDto>> GetSyncUserProjectInfosAsync(
        [FromServices] IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper,
        GetChainBlockHeightDto dto)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<UserProjectInfoIndex>, QueryContainer>>();
        mustQuery.Add(q => q.Term(i
            => i.Field(f => f.ChainId).Value(dto.ChainId)));

        if (dto.StartBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).GreaterThanOrEquals(dto.StartBlockHeight)));
        }

        if (dto.EndBlockHeight > 0)
        {
            mustQuery.Add(q => q.Range(i
                => i.Field(f => f.BlockHeight).LessThanOrEquals(dto.EndBlockHeight)));
        }

        QueryContainer Filter(QueryContainerDescriptor<UserProjectInfoIndex> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, 
            sortType: SortOrder.Ascending, sortExp: o => o.BlockHeight);
        return objectMapper.Map<List<UserProjectInfoIndex>, List<UserProjectInfoSyncDto>>(result.Item2);
    }
}