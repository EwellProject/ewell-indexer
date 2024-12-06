using AElf.CSharp.Core;

namespace EwellIndexer.Processors.Base;

public abstract class ProjectProcessorBase<TEvent> : ProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    public override string GetContractAddress(string chainId)
    {
        return chainId switch
        {
            EwellConst.TestNetSideChainId => EwellConst.EwellContractAddressTestNetSideChain,
            EwellConst.MainNetSideChainId => EwellConst.EwellContractAddressMainNetSideChain,
            _ => string.Empty
        };
    }
}