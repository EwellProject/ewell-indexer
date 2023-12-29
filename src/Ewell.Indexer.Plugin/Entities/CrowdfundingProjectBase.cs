using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class CrowdfundingProjectBase : AElfIndexerClientEntity<string>
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string ProjectId { get; set; }
    [Keyword] public string Creator { get; set; }
    [Keyword] public string CrowdFundingType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}