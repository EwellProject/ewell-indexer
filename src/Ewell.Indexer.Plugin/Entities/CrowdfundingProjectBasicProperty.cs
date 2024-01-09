using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class CrowdfundingProjectBasicProperty : CrowdfundingProjectBase
{
    public long ToRaisedAmount { get; set; }
    public long CrowdFundingIssueAmount { get; set; }
    public long PreSalePrice { get; set; }
    public long PublicSalePrice { get; set; }
    public long MinSubscription { get; set; }
    public long MaxSubscription { get; set; }
    [Keyword] public string ListMarketInfo { get; set; }
    public int LiquidityLockProportion { get; set; }
    public DateTime? UnlockTime { get; set; }
    public int FirstDistributeProportion { get; set; }
    public int RestDistributeProportion { get; set; }
    public int TotalPeriod { get; set; }
    [Keyword] public string AdditionalInfo { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsEnableWhitelist { get; set; }
    [Keyword] public string WhitelistId { get; set; }
    public long CurrentRaisedAmount { get; set; }
    public long CurrentCrowdFundingIssueAmount { get; set; }
    public long ParticipantCount { get; set; }
    public int CurrentPeriod { get; set; }
    public long PeriodDuration { get; set; }
    public bool IsBurnRestToken { get; set; }
    public long ReceivableLiquidatedDamageAmount { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public DateTime? CancelTime { get; set; }
}