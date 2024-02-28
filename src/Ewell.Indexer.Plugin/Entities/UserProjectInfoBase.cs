using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserProjectInfoBase : AElfIndexerClientEntity<string>
{
    [Keyword] public string User { get; set; }
    public long InvestAmount { get; set; }
    public long ToClaimAmount { get; set; }
    public long ActualClaimAmount { get; set; }
    public long LiquidatedDamageAmount { get; set; }
    public bool ClaimedLiquidatedDamage { get; set; }
    public DateTime? ClaimedLiquidatedDamageTime { get; set; }
    public DateTime CreateTime { get; set; }
}