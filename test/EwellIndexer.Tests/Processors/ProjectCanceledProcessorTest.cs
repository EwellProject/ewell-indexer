using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class ProjectCanceledProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await MockEventProcess(ProjectCanceled(), ProjectCanceledProcessor);
        
        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        projectIndex.IsCanceled.ShouldBeTrue();
    }
}