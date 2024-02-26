using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class DisInvestedProcessor : UserProjectProcessorBase<DisInvested>
{
    public DisInvestedProcessor(ILogger<AElfLogEventProcessorBase<DisInvested, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    protected override async Task HandleEventAsync(DisInvested eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        Logger.LogInformation("[DisInvested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[DisInvested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[DisInvested] user project info with id {id} does not exist.", userProjectId);
            return;
        }

        var disinvestAmount = eventValue.DisinvestAmount;
        var userInvestedAmount = userProjectInfo.InvestAmount;
        var totalToClaimAmount = userProjectInfo.ToClaimAmount;
        var projectReceivableLiquidatedDamage = userInvestedAmount - disinvestAmount;
        //reset amount
        userProjectInfo.InvestAmount = 0;
        userProjectInfo.ToClaimAmount = 0;
        userProjectInfo.ActualClaimAmount = 0;
        userProjectInfo.LiquidatedDamageAmount = projectReceivableLiquidatedDamage;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);

        crowdfundingProject.CurrentRaisedAmount -= userInvestedAmount;
        crowdfundingProject.ReceivableLiquidatedDamageAmount += projectReceivableLiquidatedDamage;
        crowdfundingProject.CurrentCrowdFundingIssueAmount -= totalToClaimAmount;
        if (crowdfundingProject.ParticipantCount > 0)
        {
            crowdfundingProject.ParticipantCount -= 1;
        }

        ObjectMapper.Map(context, crowdfundingProject);
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Disinvest,
            disinvestAmount, totalToClaimAmount);
        Logger.LogInformation("[DisInvested] end projectId:{projectId} user:{user} ", projectId, user);
    }
}