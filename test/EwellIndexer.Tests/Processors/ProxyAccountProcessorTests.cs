using AElf.Types;
using EwellIndexer.Entities;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class ProxyAccountProcessorTests : EwellIndexerTestBase
{
    [Fact]
    public async Task ProxyAccountCreated_Test()
    {
        await MockEventProcess(ProxyAccountCreated(), ProxyAccountCreatedProcessor);
        
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await GetIndexById<ProxyAccountIndex>(proxyAccountIndexId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressAdded_Test()
    {
        await ProxyAccountCreated_Test();
        await MockEventProcess(ProxyAccountManagementAddressAdded(), ProxyAccountManagementAddressAddedProcessor);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await GetIndexById<ProxyAccountIndex>(proxyAccountIndexId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("DDD".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressRemoved_Test()
    {
        await ProxyAccountCreated_Test();
        await MockEventProcess(ProxyAccountManagementAddressRemoved(), ProxyAccountManagementAddressRemovedProcessor);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await GetIndexById<ProxyAccountIndex>(proxyAccountIndexId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressReset_Test()
    {
        await ProxyAccountCreated_Test();
        await MockEventProcess(ProxyAccountManagementAddressReset(), ProxyAccountManagementAddressResetProcessor);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await GetIndexById<ProxyAccountIndex>(proxyAccountIndexId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("EEE".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("FFF".HexToByteArray()).ToBase58());
    }
    
}