namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordDto : UserRecordBaseDto
{
    public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBaseDto CrowdfundingProject { get; set; }
}