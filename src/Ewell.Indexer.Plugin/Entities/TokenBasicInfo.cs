using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class TokenBasicInfo
{
    [Keyword] public string Id { get; set; }
    [Keyword] public string ChainId { get; set; }
    [Keyword] public string Symbol { get; set; }
    [Keyword] public string Name { get; set; }
    public string Address { get; set; }
    public int Decimals { get; set; }
}