using Nest;

namespace Ewell.Indexer.Plugin.Entities;

public class TokenBasicInfo
{
    [Keyword] public string ChainId { get; set; }
    [Keyword] public string Symbol { get; set; }
}