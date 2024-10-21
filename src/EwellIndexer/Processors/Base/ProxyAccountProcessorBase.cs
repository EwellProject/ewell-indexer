using AElf.CSharp.Core;

namespace EwellIndexer.Processors.Base;

public abstract class ProxyAccountProcessorBase<TEvent> : ProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    public override string GetContractAddress(string chainId)
    {
        return chainId switch
        {
            EwellConst.TestNetSideChainId => EwellConst.ProxyAccountContractAddressTestNetSideChain,
            EwellConst.MainNetSideChainId => EwellConst.ProxyAccountContractAddressMainNetSideChain,
            _ => string.Empty
        };
    }
}