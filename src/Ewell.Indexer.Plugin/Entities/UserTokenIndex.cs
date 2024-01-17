using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class UserTokenIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]
    public string Address { get; set; }

    [Keyword]
    public string Symbol { get; set; }
    
    public long Balance { get; set; }
}