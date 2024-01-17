namespace Ewell.Indexer.Plugin;

public class IdGenerateHelper
{
    public static string GetId(params object[] inputs)
    {
        return inputs.JoinAsString("-");
    }
    
    public static string GetTokenInfoId(string chainId, string symbol)
    {
        return GetId(chainId, symbol);
    }

    public static string GetProjectId(string chainId, string projectHash)
    {
        return GetId(chainId, projectHash);
    }
    
    public static string GetUserProjectId(string chainId, string projectId, string userId)
    {
        return GetId(chainId, projectId, userId);
    }
    
    public static string GetNFTInfoId(string chainId, string symbol)
    {
        return GetId(chainId, symbol);
    }
    
    public static string GetUserBalanceId(string address, string chainId, string nftInfoId)
    {
        return GetId(address, chainId, nftInfoId);
    }
}