
namespace Ewell.Indexer.Plugin;

public class ContractInfoOptions
{
    public Dictionary<string, ContractInfo> ContractInfos { get; set; }
}

public class ContractInfo
{
    public string ChainId { get; set; }
    public string TokenContractAddress { get; set; }
    public string WhitelistContractAddress { get; set; }
    public string EwellContractAddress { get; set; }
}