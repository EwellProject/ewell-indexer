using AElf;
using AElf.Contracts.ProxyAccountContract;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Nethereum.Hex.HexConvertors.Extensions;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class ProxyAccountProcessorTests : EwellIndexerPluginTestBase
{
    
    private readonly IAElfIndexerClientEntityRepository<ProxyAccountIndex, LogEventInfo> 
        _proxyAccountIndexRepository;

    
    public ProxyAccountProcessorTests()
    {
        _proxyAccountIndexRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<ProxyAccountIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task ProxyAccountCreated_Test()
    {
        string chainId = Chain_AELF;
        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var logEvent = new ProxyAccountCreated()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddresses = new ManagementAddressList()
            {
                Value =
                {
                    new ManagementAddress
                    {
                        Address = Address.FromPublicKey("AAA".HexToByteArray())
                    },
                    new ManagementAddress
                    {
                        Address = Address.FromPublicKey("CCC".HexToByteArray())
                    }
                }
            },
            CreateChainId = chainId.GetHashCode()
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;

        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow
        };

        var processor = GetRequiredService<ProxyAccountCreatedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await _proxyAccountIndexRepository.GetFromBlockStateSetAsync(proxyAccountIndexId, chainId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressAdded_Test()
    {
        await ProxyAccountCreated_Test();
        string chainId = Chain_AELF;
        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var logEvent = new ProxyAccountManagementAddressAdded()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddress = new ManagementAddress
            {
                Address = Address.FromPublicKey("DDD".HexToByteArray())
            }
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;

        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow
        };

        var processor = GetRequiredService<ProxyAccountManagementAddressAddedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await _proxyAccountIndexRepository.GetFromBlockStateSetAsync(proxyAccountIndexId, chainId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("DDD".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressRemoved_Test()
    {
        await ProxyAccountCreated_Test();
        string chainId = Chain_AELF;
        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var logEvent = new ProxyAccountManagementAddressRemoved()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddress = new ManagementAddress
            {
                Address = Address.FromPublicKey("CCC".HexToByteArray())
            }
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;

        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow
        };

        var processor = GetRequiredService<ProxyAccountManagementAddressRemovedProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await _proxyAccountIndexRepository.GetFromBlockStateSetAsync(proxyAccountIndexId, chainId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
    }
    
    [Fact]
    public async Task ProxyAccountManagementAddressReset_Test()
    {
        await ProxyAccountCreated_Test();
        string chainId = Chain_AELF;
        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var logEvent = new ProxyAccountManagementAddressReset()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddresses = new ManagementAddressList()
            {
                Value =
                {
                    new ManagementAddress
                    {
                        Address = Address.FromPublicKey("EEE".HexToByteArray())
                    },
                    new ManagementAddress
                    {
                        Address = Address.FromPublicKey("FFF".HexToByteArray())
                    }
                }
            }
        };

        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;

        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow
        };

        var processor = GetRequiredService<ProxyAccountManagementAddressResetProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
        
        var proxyAccountIndexId =
            IdGenerateHelper.GetProxyAccountIndexId(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        var proxyAccountIndex =
            await _proxyAccountIndexRepository.GetFromBlockStateSetAsync(proxyAccountIndexId, chainId);
        proxyAccountIndex.Id.ShouldBe(proxyAccountIndexId);
        proxyAccountIndex.ProxyAccountAddress.ShouldBe(Address.FromPublicKey("BBB".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("CCC".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldNotContain(Address.FromPublicKey("AAA".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("EEE".HexToByteArray()).ToBase58());
        proxyAccountIndex.ManagersSet.ShouldContain(Address.FromPublicKey("FFF".HexToByteArray()).ToBase58());
    }
    
}