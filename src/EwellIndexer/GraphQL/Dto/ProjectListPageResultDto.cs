using System.Collections.Generic;

namespace EwellIndexer.GraphQL.Dto;

public class ProjectListPageResultDto
{
    public long TotalCount { get; set; }
    public List<CrowdfundingProjectDto> Data { get; set; }
}