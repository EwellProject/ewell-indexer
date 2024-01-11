using AElfIndexer.Client;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserRecordBase : AElfIndexerClientEntity<string>
{
    [Keyword] public string User { get; set; }
    public BehaviorType BehaviorType { get; set; }
    public long ToRaiseTokenAmount { get; set; }
    public long CrowdFundingIssueAmount { get; set; }
    public DateTime DateTime { get; set; }
}