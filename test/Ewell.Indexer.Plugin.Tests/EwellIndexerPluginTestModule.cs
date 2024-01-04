using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AElf.Indexing.Elasticsearch;
using AElf.Indexing.Elasticsearch.Options;
using AElf.Indexing.Elasticsearch.Services;
using AElfIndexer.BlockScan;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Client.Providers;
using Ewell.Indexer.Orleans.TestBase;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Ewell.Indexer.Plugin.Tests;

[DependsOn(
    typeof(EwellIndexerOrleansTestBaseModule),
    typeof(EwellIndexerPluginModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpAutofacModule),
    typeof(AElfIndexingElasticsearchModule))]
public class EwellIndexerPluginTestModule : AbpModule
{
    private string ClientId { get; } = "TestEwellClient";
    private string Version { get; } = "TestEwellVersion";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var mockEventbus = new Mock<IDistributedEventBus>();
        mockEventbus.Setup(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);
        context.Services.AddSingleton(mockEventbus.Object);
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<EwellIndexerPluginTestModule>(); });
        context.Services.AddSingleton<IAElfIndexerClientInfoProvider, AElfIndexerClientInfoProvider>();
        context.Services.AddSingleton<ISubscribedBlockHandler, SubscribedBlockHandler>();
        context.Services.AddTransient<IBlockChainDataHandler, LogEventDataHandler>();
        context.Services.AddTransient(typeof(IAElfIndexerClientEntityRepository<,>),
            typeof(AElfIndexerClientEntityRepository<,>));
        context.Services.AddSingleton(typeof(IBlockStateSetProvider<>), typeof(BlockStateSetProvider<>));
        context.Services.AddSingleton<IAElfClientProvider, AElfClientProvider>();
        context.Services.AddSingleton(mockEventbus.Object);

        context.Services.Configure<NodeOptions>(o =>
        {
            o.NodeConfigList = new List<NodeConfig>
            {
                new NodeConfig { ChainId = "AELF", Endpoint = "http://mainchain.io" },
                new NodeConfig { ChainId = "tDVW", Endpoint = "http://mainchain.io" }
            };
        });

        context.Services.Configure<EsEndpointOption>(options =>
        {
            options.Uris = new List<string> { "http://127.0.0.1:9200" };
        });

        context.Services.Configure<IndexSettingOptions>(options =>
        {
            options.NumberOfReplicas = 1;
            options.NumberOfShards = 1;
            options.Refresh = Refresh.True;
            options.IndexPrefix = "EwellIndexer";
        });

                
        context.Services.Configure<ContractInfoOptions>(options =>
        {
            options.ContractInfos = new Dictionary<string, ContractInfo>()
            {
                {
                    "AELF", new ContractInfo()
                    {
                        EwellContractAddress = "ewellContractAddress",
                        WhitelistContractAddress = "whitelist",
                    }
                },
                {
                    "tDVV", new ContractInfo()
                    {
                        EwellContractAddress = "ewellContractAddress",
                        WhitelistContractAddress = "whitelist",
                    }
                },
                {
                    "tDVW", new ContractInfo()
                    {
                        EwellContractAddress = "ewellContractAddress",
                        WhitelistContractAddress = "whitelist",
                    }
                },
            };
        });

        var applicationBuilder = new ApplicationBuilder(context.Services.BuildServiceProvider());
        context.Services.AddObjectAccessor<IApplicationBuilder>(applicationBuilder);
        var mockBlockScanAppService = new Mock<IBlockScanAppService>();
        mockBlockScanAppService.Setup(p => p.GetMessageStreamIdsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new List<Guid>()));
        context.Services.AddSingleton<IBlockScanAppService>(mockBlockScanAppService.Object);
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var provider = context.ServiceProvider.GetRequiredService<IAElfIndexerClientInfoProvider>();
        provider.SetClientId(ClientId);
        provider.SetVersion(Version);
        AsyncHelper.RunSync(async () =>
            await CreateIndexAsync(context.ServiceProvider)
        );
    }

    private async Task CreateIndexAsync(IServiceProvider serviceProvider)
    {
        var types = GetTypesAssignableFrom<IIndexBuild>(typeof(EwellIndexerPluginModule).Assembly);
        var elasticIndexService = serviceProvider.GetRequiredService<IElasticIndexService>();
        foreach (var t in types)
        {
            var indexName = $"{ClientId}-{Version}.{t.Name}".ToLower();
            await elasticIndexService.CreateIndexAsync(indexName, t);
        }
    }

    private List<Type> GetTypesAssignableFrom<T>(Assembly assembly)
    {
        var compareType = typeof(T);
        return assembly.DefinedTypes
            .Where(type => compareType.IsAssignableFrom(type) && !compareType.IsAssignableFrom(type.BaseType) &&
                           !type.IsAbstract && type.IsClass && compareType != type)
            .Cast<Type>().ToList();
    }

    private async Task DeleteIndexAsync(IServiceProvider serviceProvider)
    {
        var elasticIndexService = serviceProvider.GetRequiredService<IElasticIndexService>();
        var types = GetTypesAssignableFrom<IIndexBuild>(typeof(EwellIndexerPluginModule).Assembly);

        foreach (var t in types)
        {
            var indexName = $"{ClientId}-{Version}.{t.Name}".ToLower();
            await elasticIndexService.DeleteIndexAsync(indexName);
        }
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        AsyncHelper.RunSync(async () =>
            await DeleteIndexAsync(context.ServiceProvider)
        );
    }
}