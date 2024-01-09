using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserProjectInfoBase : AElfIndexerClientEntity<string>
{
    [Keyword] public string User { get; set; }
    public long InvestAmount { get; set; }
    public long ToClaimAmount { get; set; }
    public long ActualClaimAmount { get; set; }
    public DateTime CreateTime { get; set; }
}