using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class UserRecordIndex : UserRecordBase, IAeFinderEntity
{
    [Keyword] public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}