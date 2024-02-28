using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class ClaimDamageLogEventProcessor : UserProjectProcessorBase<LiquidatedDamageClaimed>
{
    public ClaimDamageLogEventProcessor(
        ILogger<AElfLogEventProcessorBase<LiquidatedDamageClaimed, LogEventInfo>> logger,
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
        var user = eventValue.User.ToBase58();
        var chainId = context.ChainId;
        Logger.LogInformation("[LiquidatedDamageClaimed] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            projectId, JsonConvert.SerializeObject(eventValue), chainId);
        try
        {
            var crowdfundingProject =
                await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
            if (crowdfundingProject == null)
            {
                Logger.LogInformation(
                    "[LiquidatedDamageClaimed] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId,
                    chainId);
                return;
            }
            
            crowdfundingProject.ReceivableLiquidatedDamageAmount -= eventValue.Amount;
            ObjectMapper.Map(context, crowdfundingProject);
            Logger.LogInformation("[LiquidatedDamageClaimed] SAVE: Id={Id}, ChainId={ChainId}", projectId, chainId);
            await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
            await UpdateUserProjectInfoAsync(context, projectId, user);
            Logger.LogInformation("[LiquidatedDamageClaimed] FINISH: Id={Id}, ChainId={ChainId}", projectId, chainId);

            await AddUserRecordAsync(context, crowdfundingProject, eventValue.User.ToBase58(),
                BehaviorType.LiquidatedDamageClaimed,
                eventValue.Amount, 0);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "[LiquidatedDamageClaimed] Exception Id={projectId}", projectId);
            throw;
        }
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[LiquidatedDamageClaimed] user project info with id {id} does not exist.",
                userProjectId);
            return;
        }

        userProjectInfo.ClaimedLiquidatedDamage = true;
        userProjectInfo.ClaimedLiquidatedDamageTime = context.BlockTime;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
    }
}