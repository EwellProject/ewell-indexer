using Ewell.Indexer.TestBase;
using Orleans.TestingHost;
using Volo.Abp.Modularity;

namespace Ewell.Indexer.Orleans.TestBase;

public abstract class EwellIndexerOrleansTestBase<TStartupModule> : EwellIndexerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected readonly TestCluster Cluster;

    public EwellIndexerOrleansTestBase()
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}