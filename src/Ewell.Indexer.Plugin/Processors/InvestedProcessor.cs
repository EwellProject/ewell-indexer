using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Contracts.Ido;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class InvestedProcessor : UserProjectProcessorBase<Invested>
{
    public InvestedProcessor(ILogger<AElfLogEventProcessorBase<Invested, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    protected override async Task HandleEventAsync(Invested eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        Logger.LogInformation("[Invested] start projectId:{projectId} user:{user} ", projectId, user);
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        if (crowdfundingProject == null)
        {
            Logger.LogInformation("[Invested] crowd funding  project with id {id} does not exist.", projectId);
            return;
        }

        var (isNewParticipant, lastClaimAmount) = await IsNewParicipantAsync(context, crowdfundingProject, user,
            eventValue.Amount, eventValue.ToClaimAmount);
        if (isNewParticipant)
        {
            crowdfundingProject.ParticipantCount += 1;
        }

        crowdfundingProject.CurrentCrowdFundingIssueAmount -= lastClaimAmount;
        crowdfundingProject.CurrentCrowdFundingIssueAmount += eventValue.ToClaimAmount;
        crowdfundingProject.CurrentRaisedAmount += eventValue.Amount;
        ObjectMapper.Map(context, crowdfundingProject);
        await CrowdfundingProjectRepository.AddOrUpdateAsync(crowdfundingProject);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Invest,
            eventValue.Amount, eventValue.ToClaimAmount);
        Logger.LogInformation("[Invested] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task<(bool, long)> IsNewParicipantAsync(LogEventContext context,
        CrowdfundingProjectIndex crowdfundingProject,
        string user, long investAmount, long toClaimAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, crowdfundingProject.Id, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);

        if (userProjectInfo == null)
        {
            userProjectInfo = new UserProjectInfoIndex()
            {
                Id = userProjectId,
                ChainId = context.ChainId,
                User = user,
                CrowdfundingProjectId = crowdfundingProject.Id,
                InvestAmount = investAmount,
                ToClaimAmount = toClaimAmount,
                CrowdfundingProject = crowdfundingProject,
                CreateTime = context.BlockTime
            };
            ObjectMapper.Map(context, userProjectInfo);
            await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
            return (true, 0);
        }

        var originInvestAmount = userProjectInfo.InvestAmount;
        userProjectInfo.InvestAmount = originInvestAmount + investAmount;
        var lastClaimAmount = userProjectInfo.ToClaimAmount;
        userProjectInfo.ToClaimAmount = toClaimAmount;
        userProjectInfo.CreateTime = context.BlockTime;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
        return (originInvestAmount == 0, lastClaimAmount);
    }
}