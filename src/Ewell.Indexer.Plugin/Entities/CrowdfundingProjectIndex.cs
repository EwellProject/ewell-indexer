using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class CrowdfundingProjectIndex : CrowdfundingProjectBasicProperty, IIndexBuild
{ 
    [Keyword] public string ToRaiseTokenId { get; set; }
    [Keyword] public string CrowdFundingIssueTokenId { get; set; }
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}