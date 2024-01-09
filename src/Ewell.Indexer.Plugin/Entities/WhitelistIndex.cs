using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class WhitelistIndex :  AElfIndexerClientEntity<string>, IIndexBuild
{
    public bool IsAvailable { get; set; }
}