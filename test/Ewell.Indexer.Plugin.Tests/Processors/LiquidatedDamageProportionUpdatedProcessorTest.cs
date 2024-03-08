using AElf;
using AElf.CSharp.Core.Extension;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.Processors;
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
    public async Task LiquidatedDamageProportionUpdatedProcessor_Test()
    {
        await MockProjectRegistered();
        
        var processor = GetRequiredService<LiquidatedDamageProportionUpdatedProcessor>();
        var expectedLiquidatedDamageProportion = 20_000000;
        var logEvent = new LiquidatedDamageProportionUpdated
        {
            ProjectId = HashHelper.ComputeFrom(Id),
            LiquidatedDamageProportion = expectedLiquidatedDamageProportion
        };
        await MockEventProcess(logEvent.ToLogEvent(), processor);
        var projectIndex = await _crowdfundingProjectRepository.GetFromBlockStateSetAsync(ProjectId, Chain_AELF);
        projectIndex.ShouldNotBeNull();
        projectIndex.LiquidatedDamageProportion.ShouldBe(expectedLiquidatedDamageProportion);
    }
}