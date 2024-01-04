namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class CrowdfundingProjectDto : CrowdfundingProjectBasicPropertyDto
{
    public TokenBasicInfoDto ToRaiseToken { get; set; }
    public TokenBasicInfoDto CrowdFundingIssueToken { get; set; }
}