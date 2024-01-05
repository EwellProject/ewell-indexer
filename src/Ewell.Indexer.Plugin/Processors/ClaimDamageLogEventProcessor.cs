using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ClaimDamageLogEventProcessor : UserProjectProcessorBase<LiquidatedDamageClaimed>
{
    public ClaimDamageLogEventProcessor(ILogger<AElfLogEventProcessorBase<LiquidatedDamageClaimed, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoOptions.ContractInfos[chainId].EwellContractAddress;
    }

    protected override async Task HandleEventAsync(LiquidatedDamageClaimed eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var chainId = context.ChainId;
        Logger.LogInformation("[LiquidatedDamageClaimed] START: Id={Id}, Event={Event}", 
            projectId, JsonConvert.SerializeObject(eventValue));
        try
        {
            var crowdfundingProject = await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
            if (crowdfundingProject == null)
            {
                Logger.LogInformation("[LiquidatedDamageClaimed] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId, chainId);
                return;
            }

            crowdfundingProject.ReceivableLiquidatedDamageAmount -= eventValue.Amount;
            ObjectMapper.Map(context, crowdfundingProject);
            
            Logger.LogInformation("[LiquidatedDamageClaimed] SAVE: Id={Id}", projectId);
            await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
            Logger.LogInformation("[LiquidatedDamageClaimed] FINISH: Id={Id}", projectId);
            
            await AddUserRecordAsync(context, crowdfundingProject, eventValue.User.ToBase58(), BehaviorType.LiquidatedDamageClaimed,
                eventValue.Amount, 0);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[LiquidatedDamageClaimed] Exception Id={projectId}", projectId);
            throw;
        }
    }
}