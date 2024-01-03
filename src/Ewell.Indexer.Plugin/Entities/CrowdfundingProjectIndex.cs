using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class CrowdfundingProjectIndex : CrowdfundingProjectBasicProperty, IIndexBuild
{
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}