using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Shouldly;
using Xunit;

namespace Ewell.Indexer.Plugin.Tests.Processors;

public class InvestedProcessorTest : EwellIndexerPluginTestBase
{
    
    private readonly IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>
        _crowdfundingProjectRepository;

    private readonly IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> _userProjectInfoRepository;
    private readonly IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> _userRecordRepository;


    public InvestedProcessorTest()
    {
        _crowdfundingProjectRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo>>();
        _userProjectInfoRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo>>();
        _userRecordRepository = GetRequiredService<IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo>>();
    }

    [Fact]
    public async Task HandleEventAsync_Test()
    {
        await MockProjectRegistered();
        
        var invested = await MockInvested();

        string chainId = Chain_AELF;
        
        var userProjectId = IdGenerateHelper.GetUserProjectId(chainId, invested.ProjectId.ToHex(), BobAddress);
        var userProjectInfoIndex = await _userProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, chainId);
        userProjectInfoIndex.ShouldNotBeNull();
        userProjectInfoIndex.InvestAmount.ShouldBe(invested.Amount);
    }
}