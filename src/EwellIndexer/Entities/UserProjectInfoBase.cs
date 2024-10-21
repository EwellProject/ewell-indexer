using System;
using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class UserProjectInfoBase : AeFinderEntity
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