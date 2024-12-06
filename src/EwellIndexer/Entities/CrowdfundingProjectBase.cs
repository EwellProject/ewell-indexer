using System;
using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class CrowdfundingProjectBase : AeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string Creator { get; set; }
    [Keyword] public string CrowdFundingType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime TokenReleaseTime { get; set; }
    public DateTime CreateTime { get; set; }
}