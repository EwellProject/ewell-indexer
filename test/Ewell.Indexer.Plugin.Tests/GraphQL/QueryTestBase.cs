using AElf;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class QueryTestBase : EwellIndexerPluginTestBase
{
    protected IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistRepository;
    protected IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo> _projectRepository;
    protected readonly IObjectMapper _objectMapper;
    protected readonly IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo> _liquidatedDamageClaimedRepository;
    protected readonly Hash whitelistId = HashHelper.ComputeFrom("test1@gmail.com");
    protected readonly Hash projectId = HashHelper.ComputeFrom("project");
    public QueryTestBase()
    {
        _objectMapper = GetRequiredService<IObjectMapper>();
        _whitelistRepository = GetRequiredService<IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo>>();
        _liquidatedDamageClaimedRepository = GetRequiredService<IAElfIndexerClientEntityRepository<LiquidatedDamageClaimedIndex, LogEventInfo>>();
        _projectRepository = GetRequiredService<IAElfIndexerClientEntityRepository<ProjectIndex, LogEventInfo>>();
    }
}