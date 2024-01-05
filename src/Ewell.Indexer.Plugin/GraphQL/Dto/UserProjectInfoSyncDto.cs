using Ewell.Indexer.Plugin.Entities;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserProjectInfoSyncDto
{
    public string CrowdfundingProjectId  { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
}