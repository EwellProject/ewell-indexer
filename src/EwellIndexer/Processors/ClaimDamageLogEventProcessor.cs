using System;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class ClaimDamageLogEventProcessor : UserProjectProcessorBase<LiquidatedDamageClaimed>
{
    public override async Task ProcessAsync(LiquidatedDamageClaimed logEvent, LogEventContext context)
    {
        var projectId = logEvent.ProjectId.ToHex();
        var user = logEvent.User?.ToBase58() ?? string.Empty;
        var chainId = context.ChainId;
        Logger.LogInformation("[LiquidatedDamageClaimed] START: Id={Id}, Event={Event}, ChainId={ChainId}",
            projectId, JsonConvert.SerializeObject(logEvent), chainId);
        try
        {
            var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
            if (crowdfundingProject == null)
            {
                Logger.LogInformation(
                    "[LiquidatedDamageClaimed] crowdfundingProject not exist: Id={Id}, ChainId={ChainId}", projectId,
                    chainId);
                return;
            }
            
            crowdfundingProject.ReceivableLiquidatedDamageAmount -= logEvent.Amount;
            Logger.LogInformation("[LiquidatedDamageClaimed] SAVE: Id={Id}, ChainId={ChainId}", projectId, chainId);
            await SaveEntityAsync(crowdfundingProject);
            await UpdateUserProjectInfoAsync(context, projectId, user);
            Logger.LogInformation("[LiquidatedDamageClaimed] FINISH: Id={Id}, ChainId={ChainId}", projectId, chainId);

            await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.LiquidatedDamageClaimed,
                logEvent.Amount, 0);
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
        var userProjectInfo = await GetEntityAsync<UserProjectInfoIndex>(userProjectId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[LiquidatedDamageClaimed] user project info with id {id} does not exist.",
                userProjectId);
            return;
        }

        userProjectInfo.ClaimedLiquidatedDamage = true;
        userProjectInfo.ClaimedLiquidatedDamageTime = context.Block.BlockTime;
        await SaveEntityAsync(userProjectInfo);
    }
}