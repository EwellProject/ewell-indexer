
namespace Ewell.Indexer.Plugin;

public class ContractInfoOptions
{
    public Dictionary<string, ContractInfo> ContractInfos { get; set; }
}
public class ContractInfo
{
    public string EwellContractAddress { get; set; }
}