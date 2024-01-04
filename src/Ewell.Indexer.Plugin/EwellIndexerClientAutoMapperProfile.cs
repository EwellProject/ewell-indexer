using AElf.Contracts.Ewell;
using AElfIndexer.Client.Handlers;
using AElf.Contracts.Whitelist;
using AutoMapper;
using Ewell.Indexer.Plugin.Entities;
using Ewell.Indexer.Plugin.GraphQL.Dto;
using Ewell.Indexer.Plugin.GraphQL;

namespace Ewell.Indexer.Plugin;

public class EwellIndexerClientAutoMapperProfile : Profile
{
    public EwellIndexerClientAutoMapperProfile()
    {
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
        CreateMap<CrowdfundingProjectIndex, CrowdfundingProjectDto>();
        
        //whitelist
        CreateMap<WhitelistIndex, WhitelistDto>();
        CreateMap<LogEventContext, WhitelistIndex>();
        CreateMap<WhitelistDisabled, WhitelistIndex>();
        CreateMap<WhitelistReenable, WhitelistIndex>();
        
        //project
        CreateMap<ProjectIndex, ProjectDto>();
        CreateMap<LogEventContext, ProjectIndex>();
        CreateMap<NewWhitelistIdSet, ProjectIndex>();
        
        //claimedDamage
        CreateMap<LiquidatedDamageClaimedIndex, DamageClaimedDto>();
        CreateMap<LogEventContext, LiquidatedDamageClaimedIndex>();
        CreateMap<LiquidatedDamageClaimed, LiquidatedDamageClaimedIndex>();
    }
}