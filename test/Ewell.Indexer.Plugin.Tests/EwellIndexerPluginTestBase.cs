using AElf;
using AElf.Contracts.Ewell;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Orleans.TestBase;
using Ewell.Indexer.Plugin.Processors;
using Ewell.Indexer.Plugin.Tests.Helper;
using Google.Protobuf.WellKnownTypes;

namespace Ewell.Indexer.Plugin.Tests;

public abstract class EwellIndexerPluginTestBase : EwellIndexerOrleansTestBase<EwellIndexerPluginTestModule>
{
    private readonly IAElfIndexerClientInfoProvider _indexerClientInfoProvider;
    public IBlockStateSetProvider<LogEventInfo> _blockStateSetLogEventInfoProvider;
    private readonly IBlockStateSetProvider<TransactionInfo> _blockStateSetTransactionInfoProvider;
    private readonly IDAppDataProvider _dAppDataProvider;
    private readonly IDAppDataIndexManagerProvider _dAppDataIndexManagerProvider;


    public static string AliceAddress = "2Pvmz2c57roQAJEtQ11fqavofdDtyD1Vehjxd7QRpQ7hwSqcF7";
    public static string BobAddress = "Lmemfcp2nB8kAvQDLxsLtQuHWgpH5gUWVmmcEkpJ2kRY9Jv25";

    public string blockHash = "dac5cd67a2783d0a3d843426c2d45f1178f4d052235a907a0d796ae4659103b1";
    public string previousBlockHash = "e38c4fb1cf6af05878657cb3f7b5fc8a5fcfb2eec19cd76b73abb831973fbf4e";
    public string transactionId = "c1e625d135171c766999274a00a7003abed24cfe59a7215aabf1472ef20a2da2";
    public long blockHeight = 120;

    public static string Chain_AELF = "AELF";
    public static string TestSymbol = "Ewell_Test";
    public static string Id = "123456";
    public static string ProjectId = HashHelper.ComputeFrom(Id).ToHex();

    public EwellIndexerPluginTestBase()
    {
        _indexerClientInfoProvider = GetRequiredService<IAElfIndexerClientInfoProvider>();
        _blockStateSetLogEventInfoProvider = GetRequiredService<IBlockStateSetProvider<LogEventInfo>>();
        _blockStateSetTransactionInfoProvider = GetRequiredService<IBlockStateSetProvider<TransactionInfo>>();
        _dAppDataProvider = GetRequiredService<IDAppDataProvider>();
        _dAppDataIndexManagerProvider = GetRequiredService<IDAppDataIndexManagerProvider>();
    }

    protected async Task<string> InitializeBlockStateSetAsync(BlockStateSet<LogEventInfo> blockStateSet, string chainId)
    {
        var key = GrainIdHelper.GenerateGrainId("BlockStateSets", _indexerClientInfoProvider.GetClientId(), chainId,
            _indexerClientInfoProvider.GetVersion());

        await _blockStateSetLogEventInfoProvider.SetBlockStateSetAsync(key, blockStateSet);
        await _blockStateSetLogEventInfoProvider.SetCurrentBlockStateSetAsync(key, blockStateSet);
        await _blockStateSetLogEventInfoProvider.SetLongestChainBlockStateSetAsync(key, blockStateSet.BlockHash);

        return key;
    }

    protected async Task<string> InitializeBlockStateSetAsync(BlockStateSet<TransactionInfo> blockStateSet,
        string chainId)
    {
        var key = GrainIdHelper.GenerateGrainId("BlockStateSets", _indexerClientInfoProvider.GetClientId(), chainId,
            _indexerClientInfoProvider.GetVersion());

        await _blockStateSetTransactionInfoProvider.SetBlockStateSetAsync(key, blockStateSet);
        await _blockStateSetTransactionInfoProvider.SetCurrentBlockStateSetAsync(key, blockStateSet);
        await _blockStateSetTransactionInfoProvider.SetLongestChainBlockStateSetAsync(key, blockStateSet.BlockHash);

        return key;
    }

    protected async Task BlockStateSetSaveDataAsync<TSubscribeType>(string key)
    {
        await _dAppDataProvider.SaveDataAsync();
        await _dAppDataIndexManagerProvider.SavaDataAsync();
        if (typeof(TSubscribeType) == typeof(TransactionInfo))
            await _blockStateSetTransactionInfoProvider.SaveDataAsync(key);
        else if (typeof(TSubscribeType) == typeof(LogEventInfo))
            await _blockStateSetLogEventInfoProvider.SaveDataAsync(key);
    }

    protected LogEventContext MockLogEventContext(long inputBlockHeight = 100, string chainId = "tDVW")
    {
        const string blockHash = "dac5cd67a2783d0a3d843426c2d45f1178f4d052235a907a0d796ae4659103b1";
        const string previousBlockHash = "e38c4fb1cf6af05878657cb3f7b5fc8a5fcfb2eec19cd76b73abb831973fbf4e";
        const string transactionId = "c1e625d135171c766999274a00a7003abed24cfe59a7215aabf1472ef20a2da2";
        var blockHeight = inputBlockHeight;
        return new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            BlockTime = DateTime.UtcNow.FromUnixTimeSeconds(1672502400),
            ExtraProperties = new Dictionary<string, string>
            {
                { "TransactionFee", "{\"ELF\": 10, \"DECIMAL\": 8}" },
                { "ResourceFee", "{\"ELF\": 10, \"DECIMAL\": 15}" }
            }
        };
    }

    protected LogEventInfo MockLogEventInfo(LogEvent logEvent)
    {
        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(logEvent);
        var logEventContext = MockLogEventContext(100);
        logEventInfo.BlockHeight = logEventContext.BlockHeight;
        logEventInfo.ChainId = logEventContext.ChainId;
        logEventInfo.BlockHash = logEventContext.BlockHash;
        logEventInfo.TransactionId = logEventContext.TransactionId;
        logEventInfo.BlockTime = DateTime.Now;
        return logEventInfo;
    }

    protected async Task<string> MockBlockState(LogEventContext logEventContext)
    {
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = logEventContext.BlockHash,
            BlockHeight = logEventContext.BlockHeight,
            Confirmed = true,
            PreviousBlockHash = logEventContext.PreviousBlockHash
        };
        return await InitializeBlockStateSetAsync(blockStateSet, logEventContext.ChainId);
    }

    protected async Task<string> MockBlockStateForTransactionInfo(LogEventContext logEventContext)
    {
        var blockStateSet = new BlockStateSet<TransactionInfo>
        {
            BlockHash = logEventContext.BlockHash,
            BlockHeight = logEventContext.BlockHeight,
            Confirmed = true,
            PreviousBlockHash = logEventContext.PreviousBlockHash
        };
        return await InitializeBlockStateSetAsync(blockStateSet, logEventContext.ChainId);
    }


    protected async Task MockProjectRegistered()
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
        var logEvent = new ProjectRegistered()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            CrowdFundingType = "1",
            StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
            EndTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Creator = Address.FromBase58(AliceAddress),
            ToRaisedAmount = 200000000,
            CrowdFundingIssueAmount = 100000000,
            PreSalePrice = 100,
            PublicSalePrice = 500,
            MinSubscription = 100,
            MaxSubscription = 1000,
            LiquidityLockProportion = 20,
            FirstDistributeProportion = 50,
            RestDistributeProportion = 50,
            TotalPeriod = 10,
            IsEnableWhitelist = false,
            ProjectCurrency = TestSymbol,
            AcceptedCurrency = "ELF"
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
            TransactionId = transactionId
        };

        var processor = GetRequiredService<ProjectRegisteredProcessor>();
        await processor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(0);
    }
}