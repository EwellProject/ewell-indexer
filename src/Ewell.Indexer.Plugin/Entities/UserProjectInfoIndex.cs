using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserProjectInfoIndex : UserProjectInfoBase, IIndexBuild
{
    [Keyword] public string CrowdfundingProjectId  { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
    public TokenBasicInfo ToRaiseToken { get; set; }
    public TokenBasicInfo CrowdFundingIssueToken { get; set; }
}