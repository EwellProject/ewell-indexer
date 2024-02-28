using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace Ewell.Indexer.Plugin.Tests.GraphQL;

public class QueryTestBase : EwellIndexerPluginTestBase
{
    protected readonly IObjectMapper _objectMapper;
    protected readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;
    protected readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> _crowdfundingProjectRepository;
    protected readonly IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo> _whitelistRepository;

    protected QueryTestBase()
    {
        _objectMapper = GetRequiredService<IObjectMapper>();
        _userRecordRepository = GetRequiredService<IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo>>();
        _crowdfundingProjectRepository = GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
        _whitelistRepository = GetRequiredService<IAElfIndexerClientEntityRepository<WhitelistIndex, LogEventInfo>>();
    }
}