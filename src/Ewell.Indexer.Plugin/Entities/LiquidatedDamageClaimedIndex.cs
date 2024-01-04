using AElf.Indexing.Elasticsearch;
using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class LiquidatedDamageClaimedIndex : IndexBase, IIndexBuild
{
    [Keyword] public string ProjectId { get; set; }
    [Keyword] public string User { get; set; }
    [Keyword] public string InvestSymbol { get; set; }
    public long Amount { get; set; }
    [Keyword] public string LastModifyTime { get; set; }
}