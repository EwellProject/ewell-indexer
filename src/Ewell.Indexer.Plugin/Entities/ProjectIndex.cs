using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class ProjectIndex : IndexBase, IIndexBuild
{
    [Keyword] public string WhitelistId { get; set; }
    [Keyword] public string LastModifyTime { get; set; }
}