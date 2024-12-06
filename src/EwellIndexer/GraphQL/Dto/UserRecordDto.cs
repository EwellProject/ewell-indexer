using EwellIndexer.Entities;

namespace EwellIndexer.GraphQL.Dto;

public class UserRecordDto : UserRecordBaseDto
{
    public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBaseDto CrowdfundingProject { get; set; }
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}