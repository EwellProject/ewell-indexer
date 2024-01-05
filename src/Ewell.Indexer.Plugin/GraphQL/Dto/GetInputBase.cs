namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class GetInputBase
{
    public string ChainId { get; set; }
    public long StartBlockHeight { get; set; }
    public long EndBlockHeight { get; set; }
    public int SkipCount { get; set; }
    public int MaxResultCount { get; set; }
}