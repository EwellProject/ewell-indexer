using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class ProjectRegisteredProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.TargetRaisedAmount.ShouldBe(200000000);
        projectIndex.RestPeriodDistributeProportion.ShouldBe(50);
        projectIndex.LiquidatedDamageProportion.ShouldBe(null);
    }
}