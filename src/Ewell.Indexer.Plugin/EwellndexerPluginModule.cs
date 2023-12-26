using AElfIndexer.Client;
using Ewell.Indexer.Plugin.GraphQL;
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
    }

    protected override string ClientId => "AElfIndexer_ewell";
    protected override string Version => "******";
}