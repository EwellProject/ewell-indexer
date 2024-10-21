using System;
using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class UserRecordBase : AeFinderEntity
{
    [Keyword] public string User { get; set; }
    public BehaviorType BehaviorType { get; set; }
    public long ToRaiseTokenAmount { get; set; }
    public long CrowdFundingIssueAmount { get; set; }
    public DateTime DateTime { get; set; }
}