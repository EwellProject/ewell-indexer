using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public class LiquidatedDamageProportionUpdatedProcessorTest : EwellIndexerTestBase
{
    [Fact]
    public async Task LiquidatedDamageProportionUpdatedProcessor_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var liquidatedDamageProportionUpdated = LiquidatedDamageProportionUpdated();
        var expectedLiquidatedDamageProportion = liquidatedDamageProportionUpdated.LiquidatedDamageProportion;
        await MockEventProcess(liquidatedDamageProportionUpdated, LiquidatedDamageProportionUpdatedProcessor);
        
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(ProjectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.LiquidatedDamageProportion.ShouldBe(expectedLiquidatedDamageProportion);
    }
}