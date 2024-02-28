namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class ProjectListPageResultDto
{
    public long TotalCount { get; set; }
    public List<CrowdfundingProjectDto> Data { get; set; }
}