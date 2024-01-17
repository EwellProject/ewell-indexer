namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class CrowdfundingProjectBaseDto : BlockInfoDto
{
    public string Id { get; set; }
    public string Creator { get; set; }
    public string CrowdFundingType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime TokenReleaseTime { get; set; }
    public DateTime CreateTime { get; set; }
}