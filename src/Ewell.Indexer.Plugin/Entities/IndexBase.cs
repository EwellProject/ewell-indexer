using AElfIndexer.Client;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class IndexBase : AElfIndexerClientEntity<string>
{
    [Keyword] public override string Id { get; set; }
}