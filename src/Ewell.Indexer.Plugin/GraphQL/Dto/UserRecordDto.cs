using Ewell.Indexer.Plugin.Entities;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordDto : UserRecordBase
{
    public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
}