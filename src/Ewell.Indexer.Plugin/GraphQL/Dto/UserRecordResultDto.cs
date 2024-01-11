namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordResultDto
{
    public long TotalCount { get; set; }
    public List<UserRecordDto> Data { get; set; }
}