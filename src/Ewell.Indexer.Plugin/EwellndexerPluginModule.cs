using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.GraphQL;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Ewell.Indexer.Plugin.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Ewell.Indexer.Plugin;

[DependsOn(typeof(AElfIndexerClientModule), typeof(AbpAutoMapperModule))]
public class EwellIndexerPluginModule : AElfIndexerClientPluginBaseModule<EwellIndexerPluginModule,
    EwellIndexerPluginSchema, Query>
{
    protected override void ConfigureServices(IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.GetConfiguration();
        Configure<ContractInfoOptions>(configuration.GetSection("ContractInfo"));
        //add processors
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, ProjectRegisteredProcessor>();
    }

    protected override string ClientId => "AElfIndexer_ewell";
    protected override string Version => "******";
}