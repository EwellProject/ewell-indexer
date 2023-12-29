using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserRecordIndex : UserRecordBase, IIndexBuild
{
    [Keyword] public string CrowdfundingProjectId { get; set; }
    public CrowdfundingProjectBase CrowdfundingProject { get; set; }
}