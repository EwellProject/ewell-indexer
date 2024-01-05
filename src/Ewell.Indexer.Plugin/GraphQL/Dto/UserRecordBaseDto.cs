using AElfIndexer.Client;
using Ewell.Indexer.Plugin.Entities;
using Nest;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class UserRecordBaseDto : BlockInfoDto
{
    public string User { get; set; }
    public BehaviorType BehaviorType { get; set; }
    public long ToRaiseTokenAmount { get; set; }
    public long CrowdFundingIssueAmount { get; set; }
    public DateTime DateTime { get; set; }
}