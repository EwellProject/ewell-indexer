using AElfIndexer.Client.GraphQL;

namespace Ewell.Indexer.Plugin.GraphQL.Dto;

public class EwellIndexerPluginSchema : AElfIndexerClientSchema<Query>
{
    public EwellIndexerPluginSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}