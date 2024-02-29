using AElf;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class ProjectRegisteredProcessorTest : EwellIndexerPluginTestBase
{
    
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;
    
    public ProjectRegisteredProcessorTest()
    {
        _crowdfundingProjectRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockProjectRegistered();
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, Chain_AELF);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.TargetRaisedAmount.ShouldBe(200000000);
        projectIndex.RestPeriodDistributeProportion.ShouldBe(50);
        projectIndex.LiquidatedDamageProportion.ShouldBe(null);
    }
}