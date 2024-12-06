using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AeFinder.Sdk;
using EwellIndexer.Entities;
using EwellIndexer.GraphQL.Dto;
using EwellIndexer.GraphQL.Input;
using GraphQL;
using Volo.Abp.ObjectMapping;

namespace EwellIndexer.GraphQL;

public class Query
{
    [Name("getProjectList")]
    public static async Task<ProjectListPageResultDto> GetProjectListAsync(
        [FromServices] IReadOnlyRepository<CrowdfundingProjectIndex> projectRepository,
        [FromServices] IReadOnlyRepository<WhitelistIndex> whitelistRepository,
        [FromServices] IObjectMapper objectMapper,
        GetProjectListInput input)
    {
        var projectQueryable = await projectRepository.GetQueryableAsync();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            projectQueryable = projectQueryable.Where(a => a.Metadata.ChainId == input.ChainId);
        }

        if (input.StartBlockHeight > 0)
        {
            projectQueryable = projectQueryable.Where(a => a.Metadata.Block.BlockHeight >= input.StartBlockHeight);
        }

        if (input.EndBlockHeight > 0)
        {
            projectQueryable = projectQueryable.Where(a => a.Metadata.Block.BlockHeight <= input.EndBlockHeight);
        }
        var count = projectQueryable.Count();
        projectQueryable = projectQueryable.Skip(input.SkipCount).Take(input.MaxResultCount)
            .OrderBy(a => a.Metadata.Block.BlockHeight);
        var list = projectQueryable.ToList();
        var projectList = objectMapper.Map<List<CrowdfundingProjectIndex>, List<CrowdfundingProjectDto>>(list);

        //whitelist
        var whitelistIds = projectList.Select(x => x.WhitelistId).ToList();
        var whitelistQueryable = await whitelistRepository.GetQueryableAsync();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            whitelistQueryable = whitelistQueryable.Where(a => a.Metadata.ChainId == input.ChainId);
        }

        if (!whitelistIds.IsNullOrEmpty())
        {
            whitelistQueryable = whitelistQueryable.Where(
                whitelistIds.Select(whitelistId => (Expression<Func<WhitelistIndex, bool>>)(o => o.Id == whitelistId))
                    .Aggregate((prev, next) => prev.Or(next)));
        }

        var whitelistList = whitelistQueryable.ToList();
        var whitelistMap = whitelistList.ToDictionary(whitelist => whitelist.Id);
        foreach (var project in projectList.Where(project => whitelistMap.ContainsKey(project.WhitelistId)))
        {
            project.IsEnableWhitelist = whitelistMap.GetOrDefault(project.WhitelistId).IsAvailable;
        }

        return new ProjectListPageResultDto
        {
            TotalCount = count,
            Data = projectList,
        };
    }
    
    [Name("getUserRecordList")]
    public static async Task<UserRecordResultDto> GetUserRecordListAsync(
        [FromServices] IReadOnlyRepository<UserRecordIndex> repository,
        [FromServices] IObjectMapper objectMapper,
        GetUserRecordInput input)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);
        }

        if (input.StartBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight >= input.StartBlockHeight);
        }

        if (input.EndBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight <= input.EndBlockHeight);
        }
        var count = queryable.Count();

        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount)
            .OrderBy(a => a.Metadata.Block.BlockHeight);
        var list = objectMapper.Map<List<UserRecordIndex>, List<UserRecordDto>>(queryable.ToList());
        return new UserRecordResultDto
        {
            TotalCount = count,
            Data = list
        };
    }
    
    [Name("getSyncUserProjectInfos")]
    public static async Task<List<UserProjectInfoSyncDto>> GetSyncUserProjectInfosAsync(
        [FromServices] IReadOnlyRepository<UserProjectInfoIndex> repository,
        [FromServices] IObjectMapper objectMapper,
        GetChainBlockHeightDto input)
    {
        var queryable = await repository.GetQueryableAsync();
        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);

        if (input.StartBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight >= input.StartBlockHeight);
        }

        if (input.EndBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight <= input.EndBlockHeight);
        }

        queryable = queryable.Skip(input.SkipCount)
            .OrderBy(a => a.Metadata.Block.BlockHeight);
        return objectMapper.Map<List<UserProjectInfoIndex>, List<UserProjectInfoSyncDto>>(queryable.ToList());
    }

    [Name("getWhitelistList")]
    public static async Task<WhitelistResultDto> GetWhitelistListAsync(
        [FromServices] IReadOnlyRepository<WhitelistIndex> repository,
        [FromServices] IObjectMapper objectMapper,
        GetWhitelistInput input)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!string.IsNullOrEmpty(input.ChainId))
        {
            queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);
        }

        if (input.StartBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight >= input.StartBlockHeight);
        }

        if (input.EndBlockHeight > 0)
        {
            queryable = queryable.Where(a => a.Metadata.Block.BlockHeight <= input.EndBlockHeight);
        }

        var count = queryable.Count();
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount)
            .OrderBy(a => a.Metadata.Block.BlockHeight);
        var list = objectMapper.Map<List<WhitelistIndex>, List<WhitelistDto>>(queryable.ToList());
        return new WhitelistResultDto
        {
            TotalCount = count,
            Data = list
        };
    }
    
     [Name("getUserTokenInfos")]
    public static async Task<List<GetUserTokensDto>> GetUserTokenInfosAsync(
        [FromServices] IReadOnlyRepository<TokenInfoIndex> tokenIndexRepository,
        [FromServices] IReadOnlyRepository<UserBalanceIndex> userBalanceIndexRepository,
        [FromServices] IReadOnlyRepository<ProxyAccountIndex> proxyAccountIndexRepository,
        [FromServices] IObjectMapper objectMapper,
        GetUserTokensInput input)
    {
        var queryable = await userBalanceIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);
        queryable = queryable.Where(a => a.Address == input.Address);
        var list = queryable.ToList();
        if (list.IsNullOrEmpty())
        {
            return new List<GetUserTokensDto>();
        }

        var tokenIds = list.Select(item => IdGenerateHelper.GetTokenInfoId(item.Metadata.ChainId, item.Symbol))
            .ToList();

        var tokenInfos = await GetTokenInfosAsync(tokenIndexRepository, tokenIds);

        var userTokensDtos = new List<GetUserTokensDto>();
        foreach (var item in list)
        {
            var id = IdGenerateHelper.GetTokenInfoId(item.Metadata.ChainId, item.Symbol);
            if (tokenInfos.TryGetValue(id, out var tokenInfo))
            {
                var isIssuer  = await CheckIsIssuer(proxyAccountIndexRepository, item.Metadata.ChainId, tokenInfo.Issuer,
                    item.Address);
                if (!isIssuer)
                {
                    continue;
                }
                var userTokensDto = objectMapper.Map<TokenInfoIndex, GetUserTokensDto>(tokenInfo);
                userTokensDto.Balance = item.Amount;
                userTokensDto.ImageUrl = tokenInfo.ExternalInfoDictionary
                    .FirstOrDefault(o => o.Key == "__nft_image_url")?.Value;
                     
                userTokensDtos.Add(userTokensDto);
            }
        }
        return userTokensDtos;
    }

    private static async Task<Dictionary<string, TokenInfoIndex>> GetTokenInfosAsync(
        [FromServices] IReadOnlyRepository<TokenInfoIndex> tokenIndexRepository,
        List<string> tokenIds)
    {
        var queryable = await tokenIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(
            tokenIds.Select(tokenId => (Expression<Func<TokenInfoIndex, bool>>)(o => o.Id == tokenId))
                .Aggregate((prev, next) => prev.Or(next)));
        
        var result = queryable.ToList();
        return queryable.ToList().IsNullOrEmpty()
            ? new Dictionary<string, TokenInfoIndex>()
            : result.ToDictionary(item => item.Id, item => item);
    }


    private static async Task<bool> CheckIsIssuer(
        [FromServices] IReadOnlyRepository<ProxyAccountIndex> proxyAccountIndexRepository,
        string chainId, string issuerAddress, string userAddress)
    {
        if (issuerAddress.Equals(userAddress))
        {
            return true;
        }

        var proxyAccountIndexId = IdGenerateHelper.GetProxyAccountIndexId(issuerAddress);
        var queryable = await proxyAccountIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(a => a.Id == proxyAccountIndexId);
        var proxyAccountIndex = queryable.SingleOrDefault();

        return proxyAccountIndex?.ManagersSet?.Contains(userAddress) ?? false;
    }
}