using AElf.Contracts.Ewell;
using AElf.Contracts.MultiToken;
using AElf.Contracts.Whitelist;
using AElfIndexer.Client.Handlers;
using AutoMapper;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.GraphQL.Dto;

namespace Ewell.Indexer.Plugin;

public class EwellIndexerClientAutoMapperProfile : Profile
{
    public EwellIndexerClientAutoMapperProfile()
    {
        CreateMap<TokenCreated, TokenInfoIndex>();
        CreateMap<LogEventContext, TokenInfoIndex>();
        CreateMap<LogEventContext, UserBalanceIndex>();
        CreateMap<ProjectRegistered, CrowdfundingProjectIndex>()
            .ForMember(des => des.Creator, opt
                => opt.MapFrom(source => source.Creator.ToBase58()
                )).ForMember(des => des.StartTime, opt
                => opt.MapFrom(source => source.StartTime.ToDateTime()
                )).ForMember(des => des.EndTime, opt
                => opt.MapFrom(source => source.EndTime.ToDateTime()
                )).ForMember(des => des.TokenReleaseTime, opt
                => opt.MapFrom(source => source.TokenReleaseTime.ToDateTime()
                )).ForMember(des => des.UnlockTime, opt
                => opt.MapFrom(source => source.UnlockTime == null ? (DateTime?)null : source.UnlockTime.ToDateTime()
                )).ForMember(des => des.WhitelistId, opt
                => opt.MapFrom(source => source.WhitelistId == null ? string.Empty : source.WhitelistId.ToHex()
                ));
        CreateMap<LogEventContext, CrowdfundingProjectIndex>();
        CreateMap<LogEventContext, UserProjectInfoIndex>();
        CreateMap<LogEventContext, UserRecordIndex>();
        CreateMap<TokenBasicInfo, TokenBasicInfoDto>();
        CreateMap<CrowdfundingProjectIndex, CrowdfundingProjectDto>();
        CreateMap<UserProjectInfoIndex, UserProjectInfoSyncDto>();
        CreateMap<UserRecordIndex, UserRecordDto>();
        CreateMap<LogEventContext, WhitelistIndex>();
        CreateMap<WhitelistReenable, WhitelistIndex>();
        CreateMap<WhitelistDisabled, WhitelistIndex>();
        CreateMap<WhitelistIndex, WhitelistDto>();
        CreateMap<CrowdfundingProjectBase, CrowdfundingProjectBaseDto>();
        CreateMap<TokenInfoIndex, GetUserTokensDto>();
    }
}