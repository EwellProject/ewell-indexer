using Nest;

namespace EwellIndexer.Entities;

public class TokenBasicInfo
{
    [Keyword] public string ChainId { get; set; }
    [Keyword] public string Symbol { get; set; }
}