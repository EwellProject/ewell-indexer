using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class UnInvestedProcessor : UserProjectProcessorBase<UnInvested>
{
    public UnInvestedProcessor(ILogger<AElfLogEventProcessorBase<UnInvested, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    protected override async Task HandleEventAsync(UnInvested eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        Logger.LogInformation("[UnInvested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[UnInvested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        if (userProjectInfo == null)
        {
            Logger.LogInformation("[UnInvested] user project info with id {id} does not exist.", userProjectId);
            return;
        }

        var unInvestAmount = eventValue.UnInvestAmount;
        var userInvestedAmount = userProjectInfo.InvestAmount;
        var totalToClaimAmount = userProjectInfo.ToClaimAmount;
        var projectReceivableLiquidatedDamage = userInvestedAmount - unInvestAmount;
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
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.UnInvest,
            unInvestAmount, totalToClaimAmount);
        Logger.LogInformation("[UnInvested] end projectId:{projectId} user:{user} ", projectId, user);
    }
}