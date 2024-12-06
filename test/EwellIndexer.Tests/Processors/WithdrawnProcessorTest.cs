using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class WithdrawnProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await MockEventProcess(Withdrawn(), WithdrawnProcessor);
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.IsWithdraw.ShouldBeTrue();
    }
}