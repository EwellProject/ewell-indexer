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
        var userProjectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(userProjectId, Chain_AELF);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(userProjectId);
    }
}