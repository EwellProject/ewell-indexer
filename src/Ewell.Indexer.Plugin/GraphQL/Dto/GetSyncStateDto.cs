using AElfIndexer;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class GetSyncStateDto
{
    public string ChainId { get; set; }
    public BlockFilterType FilterType { get; set; }
}