using Nest;

namespace EwellIndexer.Entities;

public class ExternalInfoDictionary
{
    [Keyword] public string Key { get; set; }
    [Keyword] public string Value { get; set; }
}