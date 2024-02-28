
namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserProjectInfoSyncDto : BlockInfoDto
{
    public string Id { get; set; }
    public string User { get; set; }
    public long InvestAmount { get; set; }
    public long ToClaimAmount { get; set; }
    public long ActualClaimAmount { get; set; }
    public long LiquidatedDamageAmount { get; set; }
    public bool ClaimedLiquidatedDamage { get; set; }
    public DateTime? ClaimedLiquidatedDamageTime { get; set; }
    public DateTime CreateTime { get; set; }
    public string CrowdfundingProjectId  { get; set; }
    public CrowdfundingProjectBaseDto CrowdfundingProject { get; set; }
}