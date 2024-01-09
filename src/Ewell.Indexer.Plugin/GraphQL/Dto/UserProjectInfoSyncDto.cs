
namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserProjectInfoSyncDto
{
    public string CrowdfundingProjectId  { get; set; }
    public CrowdfundingProjectBaseDto CrowdfundingProject { get; set; }
}