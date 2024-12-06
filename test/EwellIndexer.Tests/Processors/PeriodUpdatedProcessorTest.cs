using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class PeriodUpdatedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var newPeriod = 3;
        await MockEventProcess(PeriodUpdated(newPeriod), PeriodUpdatedProcessor);

        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.CurrentPeriod.ShouldBe(newPeriod);
    }
}