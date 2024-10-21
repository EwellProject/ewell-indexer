using System;
using EwellIndexer.Entities;

namespace EwellIndexer.GraphQL.Dto;

public class UserRecordBaseDto : BlockInfoDto
{
    public string Id { get; set; }
    public string User { get; set; }
    public BehaviorType BehaviorType { get; set; }
    public long ToRaiseTokenAmount { get; set; }
    public long CrowdFundingIssueAmount { get; set; }
    public DateTime DateTime { get; set; }
}