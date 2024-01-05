using AElf.Indexing.Elasticsearch;

namespace Ewell.Indexer.Plugin.Entities;

public class CrowdfundingProjectIndex : CrowdfundingProjectBasicProperty, IIndexBuild
{
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}