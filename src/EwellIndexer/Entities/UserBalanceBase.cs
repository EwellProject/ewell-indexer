using System;
using AeFinder.Sdk.Entities;
using Nest;

namespace EwellIndexer.Entities;

public class UserBalanceBase : AeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    
    //userAccount Address
    [Keyword] public string Address { get; set; }
    
    public long Amount { get; set; }
    
    [Keyword] public string Symbol { get; set; }

    public DateTime ChangeTime { get; set; }
}