using AElf;
using EwellIndexer.Entities;
using Shouldly;
using Xunit;

namespace EwellIndexer.Processors;

public sealed class WhitelistLogEventProcessorTests : EwellIndexerTestBase
{
    [Fact]
    public async Task HandleNewWhitelistIdSetAsync_Test()
    {
        await MockEventProcess(ProjectRegistered(), ProjectRegisteredProcessor);
        var projectId = HashHelper.ComputeFrom(Id);
        var newWhitelistIdSet = NewWhitelistIdSet(WhitelistId, projectId);
        await MockEventProcess(newWhitelistIdSet, NewWhitelistIdSetLogEventProcessor);
        
        // normal test
        var projectIndex = await GetIndexById<CrowdfundingProjectIndex>(ProjectId);
        projectIndex.ShouldNotBeNull();
        projectIndex.WhitelistId.ShouldBe(WhitelistId.ToHex());
        
        // try-catch
        newWhitelistIdSet.WhitelistId = null;
        await MockEventProcess(newWhitelistIdSet, NewWhitelistIdSetLogEventProcessor);

        // errorProjectId
        var errorProjectId = HashHelper.ComputeFrom("ERROR");
        newWhitelistIdSet.ProjectId = errorProjectId;
        projectIndex = await GetIndexById<CrowdfundingProjectIndex>(errorProjectId.ToHex());
        projectIndex.ShouldBeNull();
        
        // contract address
        var address = NewWhitelistIdSetLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe(EwellConst.EwellContractAddressMainNetSideChain);
    }
    
    [Fact]
    public async Task HandleWhitelistDisableAsync_Test()
    {
        await WhitelistDisabledLogEventProcessor.ProcessAsync(GenerateLogEventContext(WhitelistDisabled()));
        var whitelist = await GetIndexById<WhitelistIndex>(WhitelistId.ToHex());
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(false);
        
        // contract address
        var address = WhitelistDisabledLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe(EwellConst.WhitelistContractAddressMainNetSideChain);
    }
    
    [Fact]
    public async Task HandleWhitelistReenableAsync_Test()
    {
        await WhitelistReenableLogEventProcessor.ProcessAsync(GenerateLogEventContext(WhitelistReenable()));
        
        // normal test
        var whitelist = await GetIndexById<WhitelistIndex>(WhitelistId.ToHex());
        whitelist.ShouldNotBeNull();
        whitelist.Id.ShouldBe(WhitelistId.ToHex());
        whitelist.IsAvailable.ShouldBe(true);
        
        // contract address
        var address = WhitelistReenableLogEventProcessor.GetContractAddress(Chain_AELF);
        address.ShouldBe(EwellConst.WhitelistContractAddressMainNetSideChain);
    }
}