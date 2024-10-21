using AeFinder.Sdk.Entities;

namespace EwellIndexer.Entities;

public class CrowdfundingProjectIndex : CrowdfundingProjectBasicProperty, IAeFinderEntity
{
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}