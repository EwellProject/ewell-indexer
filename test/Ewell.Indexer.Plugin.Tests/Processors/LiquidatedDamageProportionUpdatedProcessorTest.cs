using AElf;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class LiquidatedDamageProportionUpdatedProcessorTest : EwellIndexerPluginTestBase
{
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    public LiquidatedDamageProportionUpdatedProcessorTest()
    {
        _crowdfundingProjectRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
    }
    
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockProjectRegistered();
        
    }
}