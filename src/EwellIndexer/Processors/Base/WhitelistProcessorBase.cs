using AElf.CSharp.Core;

namespace EwellIndexer.Processors.Base;

public abstract class WhitelistProcessorBase<TEvent> : ProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    public override string GetContractAddress(string chainId)
    {
        return chainId switch
        {
            EwellConst.TestNetSideChainId => EwellConst.WhitelistContractAddressTestNetSideChain,
            EwellConst.MainNetSideChainId => EwellConst.WhitelistContractAddressMainNetSideChain,
            _ => string.Empty
        };
    }
}