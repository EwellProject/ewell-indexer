namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class WhitelistResultDto
{
    public long TotalCount { get; set; }
    public List<WhitelistDto> Data { get; set; }
}