using AElfIndexer.Client.GraphQL;

namespace Ewell.Indexer.Plugin.GraphQL;

public class EwellIndexerPluginSchema : AElfIndexerClientSchema<Query>
{
    public EwellIndexerPluginSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}