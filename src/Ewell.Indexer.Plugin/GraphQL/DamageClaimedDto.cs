namespace Ewell.Indexer.Plugin.GraphQL;

public class DamageClaimedDto : BaseDto
{
    public string User { get; set; }
    public string ProjectId { get; set; }
    public string InvestSymbol { get; set; }
    public long Amount { get; set; }
}