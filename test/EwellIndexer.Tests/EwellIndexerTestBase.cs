using AeFinder.App.TestBase;
using AeFinder.Sdk;
using AeFinder.Sdk.Processor;
using AElf;
using AElf.Contracts.MultiToken;
using AElf.Contracts.ProxyAccountContract;
using AElf.Contracts.Whitelist;
using AElf.CSharp.Core;
using AElf.Types;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Hex.HexConvertors.Extensions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace EwellIndexer;

public abstract class EwellIndexerTestBase: AeFinderAppTestBase<EwellIndexerTestModule>
{
    //2N9DJYUUruS7bFqRyKMvafA75qTWgqpWcB78nNZzpmxHrMv4D
    public static string AliceAddress = "2N9DJYUUruS7bFqRyKMvafA75qTWgqpWcB78nNZzpmxHrMv4D";
    
    //2fbCtXNLVD2SC4AD6b8nqAkHtjqxRCfwvciX4MyH6257n8Gf63
    public static string BobAddress = "2fbCtXNLVD2SC4AD6b8nqAkHtjqxRCfwvciX4MyH6257n8Gf63";

    public static string VirtualAddress = "aLyxCJvWMQH6UEykTyeWAcYss9baPyXkrMQ37BHnUicxD2LL3";

    public string blockHash = "dac5cd67a2783d0a3d843426c2d45f1178f4d052235a907a0d796ae4659103b1";
    public string previousBlockHash = "e38c4fb1cf6af05878657cb3f7b5fc8a5fcfb2eec19cd76b73abb831973fbf4e";
    public string transactionId = "4e07408562bedb8b60ce05c1decfe3ad16b72230967de01f640b7e4729b49fce";
    public long blockHeight = 100;
    public static string Chain_AELF = "tDVV";
    public static string TestSymbol = "Ewell_Test";
    public static string Id = "123456555111";
    public static string ProjectId = HashHelper.ComputeFrom(Id).ToHex();
    public static Hash WhitelistId = HashHelper.ComputeFrom("whitelistId");
    
    protected readonly IAbpLazyServiceProvider LazyServiceProvider;
    protected readonly ProjectRegisteredProcessor ProjectRegisteredProcessor;
    protected readonly AdditionalInfoUpdatedProcessor AdditionalInfoUpdatedProcessor;
    protected readonly InvestedProcessor InvestedProcessor;
    protected readonly DisInvestedProcessor DisInvestedProcessor;
    protected readonly ClaimDamageLogEventProcessor ClaimDamageLogEventProcessor;
    protected readonly ClaimedProcessor ClaimedProcessor;
    protected readonly LiquidatedDamageProportionUpdatedProcessor LiquidatedDamageProportionUpdatedProcessor;
    protected readonly PeriodUpdatedProcessor PeriodUpdatedProcessor;
    protected readonly ProjectCanceledProcessor ProjectCanceledProcessor;
    protected readonly ProxyAccountCreatedProcessor ProxyAccountCreatedProcessor;
    protected readonly ProxyAccountManagementAddressAddedProcessor ProxyAccountManagementAddressAddedProcessor;
    protected readonly ProxyAccountManagementAddressRemovedProcessor ProxyAccountManagementAddressRemovedProcessor;
    protected readonly ProxyAccountManagementAddressResetProcessor ProxyAccountManagementAddressResetProcessor;
    protected readonly RefundedProcessor RefundedProcessor;
    protected readonly TokenCreatedLogEventProcessor TokenCreatedLogEventProcessor;
    protected readonly TokenBurnedEventProcessor TokenBurnedEventProcessor;
    protected readonly TokenCrossChainReceivedProcessor TokenCrossChainReceivedProcessor;
    protected readonly TokenCrossChainTransferredProcessor TokenCrossChainTransferredProcessor;
    protected readonly TokenIssuedEventProcessor TokenIssuedEventProcessor;
    protected readonly TokenTransferredLogEventProcessor TokenTransferredLogEventProcessor;
    protected readonly NewWhitelistIdSetLogEventProcessor NewWhitelistIdSetLogEventProcessor;
    protected readonly WhitelistDisabledLogEventProcessor WhitelistDisabledLogEventProcessor;
    protected readonly WhitelistReenableLogEventProcessor WhitelistReenableLogEventProcessor;
    protected readonly WithdrawnProcessor WithdrawnProcessor;
    
    protected readonly IReadOnlyRepository<CrowdfundingProjectIndex> CrowdfundingProjectIndexRepository;
    protected readonly IReadOnlyRepository<WhitelistIndex> WhitelistIndexRepository;
    protected readonly IReadOnlyRepository<UserRecordIndex> UserRecordIndexRepository;
    
    // mapper
    protected readonly IObjectMapper ObjectMapper;
    
    public EwellIndexerTestBase()
    {
        LazyServiceProvider = GetRequiredService<IAbpLazyServiceProvider>();
        ProjectRegisteredProcessor = GetRequiredService<ProjectRegisteredProcessor>();
        AdditionalInfoUpdatedProcessor = GetRequiredService<AdditionalInfoUpdatedProcessor>();
        InvestedProcessor = GetRequiredService<InvestedProcessor>();
        DisInvestedProcessor = GetRequiredService<DisInvestedProcessor>();
        ClaimDamageLogEventProcessor = GetRequiredService<ClaimDamageLogEventProcessor>();
        ClaimedProcessor = GetRequiredService<ClaimedProcessor>();
        LiquidatedDamageProportionUpdatedProcessor = GetRequiredService<LiquidatedDamageProportionUpdatedProcessor>();
        PeriodUpdatedProcessor = GetRequiredService<PeriodUpdatedProcessor>();
        ProjectCanceledProcessor = GetRequiredService<ProjectCanceledProcessor>();
        ProxyAccountCreatedProcessor = GetRequiredService<ProxyAccountCreatedProcessor>();
        ProxyAccountManagementAddressAddedProcessor = GetRequiredService<ProxyAccountManagementAddressAddedProcessor>();
        ProxyAccountManagementAddressRemovedProcessor = GetRequiredService<ProxyAccountManagementAddressRemovedProcessor>();
        ProxyAccountManagementAddressResetProcessor = GetRequiredService<ProxyAccountManagementAddressResetProcessor>();
        RefundedProcessor = GetRequiredService<RefundedProcessor>();
        TokenCreatedLogEventProcessor = GetRequiredService<TokenCreatedLogEventProcessor>();
        TokenBurnedEventProcessor = GetRequiredService<TokenBurnedEventProcessor>();
        TokenCrossChainReceivedProcessor = GetRequiredService<TokenCrossChainReceivedProcessor>();
        TokenCrossChainTransferredProcessor = GetRequiredService<TokenCrossChainTransferredProcessor>();
        TokenIssuedEventProcessor = GetRequiredService<TokenIssuedEventProcessor>();
        TokenTransferredLogEventProcessor = GetRequiredService<TokenTransferredLogEventProcessor>();
        NewWhitelistIdSetLogEventProcessor = GetRequiredService<NewWhitelistIdSetLogEventProcessor>();
        WhitelistDisabledLogEventProcessor = GetRequiredService<WhitelistDisabledLogEventProcessor>();
        WhitelistReenableLogEventProcessor = GetRequiredService<WhitelistReenableLogEventProcessor>();
        WithdrawnProcessor = GetRequiredService<WithdrawnProcessor>();
        
        CrowdfundingProjectIndexRepository = GetRequiredService<IReadOnlyRepository<CrowdfundingProjectIndex>>();
        WhitelistIndexRepository = GetRequiredService<IReadOnlyRepository<WhitelistIndex>>();
        UserRecordIndexRepository = GetRequiredService<IReadOnlyRepository<UserRecordIndex>>();
        
        ObjectMapper = GetRequiredService<IObjectMapper>();
    }

    protected async Task MockEventProcess<TEvent>(TEvent logEvent, LogEventProcessorBase<TEvent> processor) where TEvent : IEvent<TEvent>, new()
    {
        await processor.ProcessAsync(logEvent, GenerateLogEventContext(logEvent));
    }
    
    protected async Task<TEntity> GetIndexById<TEntity>(string id)
    {
        return await LazyServiceProvider.GetRequiredService<IRepository<TEntity>>().GetAsync(id);
    }

    protected ProjectRegistered ProjectRegistered() 
    {
        return new ProjectRegistered
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            CrowdFundingType = "1",
            StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
            EndTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1)),
            TokenReleaseTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Creator = Address.FromBase58(AliceAddress),
            TargetRaisedAmount = 200000000,
            CrowdFundingIssueAmount = 100000000,
            PreSalePrice = 100,
            PublicSalePrice = 500,
            MinSubscription = 100,
            MaxSubscription = 1000,
            LiquidityLockProportion = 20,
            FirstDistributeProportion = 50,
            RestPeriodDistributeProportion = 50,
            TotalPeriod = 10,
            IsEnableWhitelist = true,
            WhitelistId = WhitelistId,
            ProjectSymbol = TestSymbol,
            AcceptedSymbol = "ELF",
            VirtualAddress = Address.FromBase58(VirtualAddress)
        };
    }
    
    protected AdditionalInfoUpdated AdditionalInfoUpdated() 
    {
        var newAdditionalInfo = new AdditionalInfo
        {
            Data =
            {
                { "1", "for value1" }, { "2", "for value2" }
            }
        };
        return new AdditionalInfoUpdated()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            
            AdditionalInfo = newAdditionalInfo
        };
    }
    
    protected Invested Invested() 
    {
        return new Invested()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            Amount = 1000,
            TotalAmount = 1000,
            ToClaimAmount = 2000
        };
    }
    
    protected DisInvested DisInvested() 
    {
        return new DisInvested()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            DisinvestedAmount = 800,
            TotalAmount = 0
        };
    }
    
    protected LiquidatedDamageClaimed LiquidatedDamageClaimed(Hash projectId, Address user, long amount, string investSymbol) 
    {
        return new LiquidatedDamageClaimed
        {
            ProjectId = projectId,
            User = user,
            Amount = amount,
            InvestSymbol = investSymbol
        };
    }
    
    protected Claimed Claimed() 
    {
        return new Claimed()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            ProjectSymbol = TestSymbol,
            Amount = 2000,
            TotalPeriod = 1,
            LatestPeriod = 1
        };
    }
    
    protected LiquidatedDamageProportionUpdated LiquidatedDamageProportionUpdated() 
    {
        var expectedLiquidatedDamageProportion = 20_000000;
        return new LiquidatedDamageProportionUpdated
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            LiquidatedDamageProportion = expectedLiquidatedDamageProportion
        };
    }
    
    protected PeriodUpdated PeriodUpdated(int newPeriod) 
    {
        return new PeriodUpdated()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            NewPeriod = newPeriod
        };
    }
    
    protected ProjectCanceled ProjectCanceled() 
    {
        return new ProjectCanceled()
        {
            ProjectId = HashHelper.ComputeFrom(Id)
        };
    }
    
    protected ProxyAccountCreated ProxyAccountCreated() 
    {
        return new ProxyAccountCreated()
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
            CreateChainId = ChainId.GetHashCode()
        };
    }
    
    protected ProxyAccountManagementAddressAdded ProxyAccountManagementAddressAdded() 
    {
        return new ProxyAccountManagementAddressAdded()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddress = new ManagementAddress
            {
                Address = Address.FromPublicKey("DDD".HexToByteArray())
            }
        };
    }
    
    protected ProxyAccountManagementAddressRemoved ProxyAccountManagementAddressRemoved() 
    {
        return new ProxyAccountManagementAddressRemoved()
        {
            ProxyAccountAddress = Address.FromPublicKey("BBB".HexToByteArray()),
            ProxyAccountHash = Hash.Empty,
            ManagementAddress = new ManagementAddress
            {
                Address = Address.FromPublicKey("CCC".HexToByteArray())
            }
        };
    }
    
    protected ProxyAccountManagementAddressReset ProxyAccountManagementAddressReset() 
    {
        return new ProxyAccountManagementAddressReset()
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
    }
    
    protected ReFunded ReFunded() 
    {
        return new ReFunded()
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            User = Address.FromBase58(BobAddress),
            InvestSymbol = TestSymbol,
            Amount = 1000
        };
    }
    
    protected TokenCreated TokenCreated(string symbol)
    {
        const string tokenName = "Token for test";
        const long totalSupply = 8;
        const int decimals = 8;
        const bool isBurnable = true;
        const int issueChainId = 9992731;

        return new TokenCreated()
        {
            Symbol = symbol,
            TokenName = tokenName,
            TotalSupply = totalSupply,
            Decimals = decimals,
            Issuer = Address.FromPublicKey("AAA".HexToByteArray()),
            IsBurnable = isBurnable,
            IssueChainId = issueChainId,
            ExternalInfo = new ExternalInfo
            {
                Value =
                {
                    {
                        "__seed_owned_symbol",
                        "WILLTESTHH"
                    },
                    {
                        "__nft_image_url",
                        ""
                    }
                }
            }
        };
    }
    
    protected Burned Burned(string symbol, long burnedAmount) 
    {
        return new Burned()
        {
            Symbol = symbol,
            Amount = burnedAmount,
            Burner = Address.FromPublicKey("BBB".HexToByteArray())
        };
    }
    
    protected CrossChainReceived CrossChainReceived(string symbol, long transferAmount) 
    {
        return new CrossChainReceived()
        {
            Symbol = symbol,
            Amount = transferAmount,
            To = Address.FromPublicKey("BBB".HexToByteArray())
        };
    }
    
    protected CrossChainReceived CrossChainReceived1(string symbol, long transferAmount) 
    {
        return new CrossChainReceived()
        {
            Symbol = symbol,
            Amount = transferAmount,
            From = Address.FromPublicKey("AAA".HexToByteArray()),
            To = Address.FromPublicKey("BBB".HexToByteArray())
        };
    }
    
    protected Issued Issued(string symbol, long issuedAmount) 
    {
        return new Issued()
        {
            Symbol = symbol,
            Amount = issuedAmount,
            To = Address.FromPublicKey("BBB".HexToByteArray())
        };
    }
    
    protected Transferred Transferred(string symbol, long transferAmount) 
    {
        return new Transferred()
        {
            Symbol = symbol,
            Amount = transferAmount,
            From = Address.FromPublicKey("AAA".HexToByteArray()),
            To = Address.FromPublicKey("BBB".HexToByteArray())
        };
    }
    
    protected NewWhitelistIdSet NewWhitelistIdSet(Hash whitelistId, Hash projectId) 
    {
        return new NewWhitelistIdSet
        {
            WhitelistId = whitelistId,
            ProjectId = projectId,
        };
    }
    
    protected WhitelistDisabled WhitelistDisabled() 
    {
        return new WhitelistDisabled
        {
            WhitelistId = WhitelistId,
            IsAvailable = false,
        };
    }
    
    protected WhitelistReenable WhitelistReenable() 
    {
        return new WhitelistReenable()
        {
            WhitelistId = WhitelistId,
            IsAvailable = true,
        };
    }
    
    protected Withdrawn Withdrawn() 
    {
        return new Withdrawn()
        {
            ProjectId = HashHelper.ComputeFrom(Id)
        };
    }
    
}