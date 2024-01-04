using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class WhitelistIndex : IndexBase, IIndexBuild
{
    public bool IsAvailable { get; set; }
    [Keyword] public string LastModifyTime { get; set; }
}