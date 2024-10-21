using System;
using AeFinder.Sdk;

namespace EwellIndexer.GraphQL;

public class AeIndexerSchema : AppSchema<Query>
{
    public AeIndexerSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}