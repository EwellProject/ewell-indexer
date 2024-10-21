using AElf;
using AElf.CSharp.Core.Extension;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class AdditionalInfoUpdatedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        await MockEventProcess(AdditionalInfoUpdated(), AdditionalInfoUpdatedProcessor);

        var projectId = HashHelper.ComputeFrom(Id).ToHex();
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(projectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.Id.ShouldBe(projectId);
        
        var newAdditionalInfoDic =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(projectIndex.AdditionalInfo);
        newAdditionalInfoDic?.Count.ShouldBe(2);
        newAdditionalInfoDic?["1"].ShouldBe("for value1");
        newAdditionalInfoDic?["2"].ShouldBe("for value2");
    }
}