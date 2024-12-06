using System.Linq;
using System.Threading.Tasks;
using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.Processors.Base;
using Newtonsoft.Json;

namespace EwellIndexer.Processors;

public class ProjectRegisteredProcessor : ProjectProcessorBase<ProjectRegistered>
{
    public override async Task ProcessAsync(ProjectRegistered logEvent, LogEventContext context)
    {
        var chainId = context.ChainId;
        var projectId = logEvent.ProjectId.ToHex();
        Logger.LogInformation("[ProjectRegistered] start projectId:{projectId} chainId:{chainId} ", projectId, chainId);
        var crowdfundingProject = await GetEntityAsync<CrowdfundingProjectIndex>(projectId);
        if (crowdfundingProject != null)
        {
            Logger.LogInformation(
                "[ProjectRegistered] crowd funding  project with id {id} chainId {chainId} has existed.", projectId,
                chainId);
            return;
        }

        var additionalInformation = logEvent.AdditionalInfo != null
            ? JsonConvert.SerializeObject(logEvent.AdditionalInfo.Data)
            : string.Empty;
        var marketInformation = logEvent.ListMarketInfo != null
            ? JsonConvert.SerializeObject(logEvent.ListMarketInfo.Data.Select(l => new
            {
                Market = l.Market.ToBase58(), l.Weight
            }).ToList())
            : string.Empty;
        var projectIndex = ObjectMapper.Map<ProjectRegistered, CrowdfundingProjectIndex>(logEvent);
        projectIndex.Id = projectId;
        projectIndex.ListMarketInfo = marketInformation;
        projectIndex.AdditionalInfo = additionalInformation;
        projectIndex.IsCanceled = false;
        projectIndex.ToRaiseToken = new TokenBasicInfo { ChainId = chainId, Symbol = logEvent.AcceptedSymbol };
        projectIndex.CrowdFundingIssueToken = new TokenBasicInfo { ChainId = chainId, Symbol = logEvent.ProjectSymbol };
        projectIndex.CreateTime = context.Block.BlockTime;
        await SaveEntityAsync(projectIndex);
        Logger.LogInformation("[ProjectRegistered] end projectId:{projectId} chainId:{chainId} ", projectId, chainId);
    }
}