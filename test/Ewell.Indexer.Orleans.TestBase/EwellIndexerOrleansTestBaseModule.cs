using Ewell.Indexer.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ewell.Indexer.Orleans.TestBase;

[DependsOn(typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(EwellIndexerTestBaseModule)
)]
public class EwellIndexerOrleansTestBaseModule : AbpModule
{
    private ClusterFixture _fixture;

    public override void ConfigureServices(ServiceConfigurationContext context)
    { 
        var _fixture = new ClusterFixture();
        context.Services.AddSingleton<ClusterFixture>(_fixture);
        context.Services.AddSingleton<IClusterClient>(sp => _fixture.Cluster.Client);
    }
}