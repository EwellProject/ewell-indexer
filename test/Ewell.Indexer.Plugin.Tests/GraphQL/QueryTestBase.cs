using AElf;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class QueryTestBase : EwellIndexerPluginTestBase
{
    protected readonly IObjectMapper _objectMapper;
    protected readonly Hash whitelistId = HashHelper.ComputeFrom("test1@gmail.com");
    protected readonly Hash projectId = HashHelper.ComputeFrom("project");
    public QueryTestBase()
    {
        _objectMapper = GetRequiredService<IObjectMapper>();
    }
}