using AElf.Contracts.Ewell;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Ewell.Indexer.Plugin.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.ObjectMapping;

namespace Ewell.Indexer.Plugin.Processors;

public class RefundedProcessor : UserProjectProcessorBase<ReFunded>
{
    public RefundedProcessor(ILogger<AElfLogEventProcessorBase<ReFunded, LogEventInfo>> logger,
        IObjectMapper objectMapper,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IAElfIndexerClientEntityRepository<CrowdfundingProjectIndex, LogEventInfo> crowdfundingProjectRepository,
        IAElfIndexerClientEntityRepository<UserProjectInfoIndex, LogEventInfo> userProjectInfoRepository,
        IAElfIndexerClientEntityRepository<UserRecordIndex, LogEventInfo> userRecordRepository) :
        base(logger, objectMapper, contractInfoOptions, crowdfundingProjectRepository, userProjectInfoRepository,
            userRecordRepository)
    {
    }

    protected override async Task HandleEventAsync(ReFunded eventValue, LogEventContext context)
    {
        var projectId = eventValue.ProjectId.ToHex();
        var user = eventValue.User.ToBase58();
        Logger.LogInformation("[ReFunded] start projectId:{projectId} user:{user} ", projectId, user);
        var refundAmount = eventValue.Amount;
        var crowdfundingProject = await UpdateProjectAsync(context, projectId, refundAmount);
        await UpdateUserProjectInfoAsync(context, projectId, user, refundAmount);
        await AddUserRecordAsync(context, crowdfundingProject, user, BehaviorType.Refund,
            refundAmount, 0);
        Logger.LogInformation("[ReFunded] end projectId:{projectId} user:{user} ", projectId, user);
    }

    private async Task<CrowdfundingProjectIndex> UpdateProjectAsync(LogEventContext context, string projectId,
        long refundAmount)
    {
        var crowdfundingProject =
            await CrowdfundingProjectRepository.GetFromBlockStateSetAsync(projectId, context.ChainId);
        // crowdfundingProject.CurrentRaisedAmount -= refundAmount;
        // return await _crowdfundingProjectsRepository.UpdateAsync(crowdfundingProject);
        return crowdfundingProject;
    }

    private async Task UpdateUserProjectInfoAsync(LogEventContext context, string projectId, string user,
        long refundAmount)
    {
        var userProjectId = IdGenerateHelper.GetUserProjectId(context.ChainId, projectId, user);
        var userProjectInfo =
            await UserProjectInfoRepository.GetFromBlockStateSetAsync(userProjectId, context.ChainId);
        userProjectInfo.InvestAmount -= refundAmount;
        userProjectInfo.ToClaimAmount = 0;
        ObjectMapper.Map(context, userProjectInfo);
        await UserProjectInfoRepository.AddOrUpdateAsync(userProjectInfo);
    }
}