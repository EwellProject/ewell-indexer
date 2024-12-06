using AeFinder.Sdk.Entities;

namespace EwellIndexer.Entities;

public class WhitelistIndex :  AeFinderEntity, IAeFinderEntity
{
    public bool IsAvailable { get; set; }
}