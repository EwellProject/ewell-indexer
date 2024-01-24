using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class ProxyAccountIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] public override string Id { get; set; }

    public string ProxyAccountAddress { get; set; }
    
    public HashSet<string> ManagersSet { get; set; }
    
    public DateTime CreateTime { get; set; }
}