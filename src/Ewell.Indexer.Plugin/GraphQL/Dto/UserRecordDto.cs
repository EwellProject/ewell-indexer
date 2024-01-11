using Ewell.Indexer.Plugin.Entities;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordDto : UserRecordBaseDto
{
    public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBaseDto CrowdfundingProject { get; set; }
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}