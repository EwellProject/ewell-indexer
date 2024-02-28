using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class WithdrawnProcessor : ProjectProcessorBase<Withdrawn>
{
    public WithdrawnProcessor(
        ILogger<AElfLogEventProcessorBase<Withdrawn, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IJsonSerializer jsonSerializer,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository) :
        base(logger, objectMapper, contractInfoOptions, jsonSerializer, crowdfundingProjectRepository)
    {
    }

    protected override async Task HandleEventAsync(Withdrawn eventValue, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = eventValue.ProjectId.ToHex();
        Logger.LogInformation("[Withdrawn] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation(
                "[Withdrawn] crowd funding project with id {id} chainId {chainId} has not existed.", projectId,
                chainId);
            return;
        }
        ObjectMapper.Map(context, crowdfundingProject);
        crowdfundingProject.IsWithdraw = true;
        crowdfundingProject.WithdrawTime = context.BlockTime;
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        Logger.LogInformation("[Withdrawn] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}