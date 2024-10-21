using Nest;

namespace EwellIndexer.Entities;

public class ExternalInfoDictionary
{
    [Text(Ignore = true)] public string Key { get; set; }
    [Text(Ignore = true)] public string Value { get; set; }
}