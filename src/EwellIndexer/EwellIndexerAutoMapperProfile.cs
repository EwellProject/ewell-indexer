using System;
using System.Collections.Generic;
using System.Linq;
using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using AElf.Contracts.ProxyAccountContract;
using AElf.Contracts.Whitelist;
using AutoMapper;
using Ewell.Contracts.Ido;
using EwellIndexer.Entities;
using EwellIndexer.GraphQL.Dto;

namespace EwellIndexer;

public class EwellIndexerAutoMapperProfile : Profile
{
    public EwellIndexerAutoMapperProfile()
    {
        CreateMap<TokenCreated, TokenInfoIndex>();
        CreateMap<LogEventContext, TokenInfoIndex>();
        CreateMap<LogEventContext, UserBalanceIndex>();
        CreateMap<ProjectRegistered, CrowdfundingProjectIndex>()
            .ForMember(des => des.Creator, opt
                => opt.MapFrom(source => source.Creator.ToBase58()
                )).ForMember(des => des.VirtualAddress, opt
                => opt.MapFrom(source => source.VirtualAddress.ToBase58()
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
                )).ForMember(des => des.LiquidatedDamageProportion, opt
                => opt.MapFrom(source => source.LiquidatedDamageProportion != null ? source.LiquidatedDamageProportion.Value : (int?)null
                ));
        // agent
        CreateMap<ProxyAccountCreated, ProxyAccountIndex>()
            .ForMember(d => d.ProxyAccountAddress,
                opt => opt.MapFrom(d =>
                    d.ProxyAccountAddress.ToBase58()))
            .ForMember(d => d.ManagersSet,
                opt => opt.MapFrom(d =>
                    d.ManagementAddresses.Value.Select(item => item.Address.ToBase58()).ToHashSet()));
        CreateMap<ProxyAccountManagementAddressAdded, ProxyAccountIndex>();
        CreateMap<ProxyAccountManagementAddressRemoved, ProxyAccountIndex>();
        CreateMap<ProxyAccountManagementAddressReset, ProxyAccountIndex>().ForMember(d => d.ProxyAccountAddress,
                opt => opt.MapFrom(d =>
                    d.ProxyAccountAddress.ToBase58()))
            .ForMember(d => d.ManagersSet,
                opt => opt.MapFrom(d =>
                    d.ManagementAddresses.Value.Select(item => item.Address.ToBase58()).ToHashSet()));
        CreateMap<LogEventContext, ProxyAccountIndex>();
        CreateMap<LogEventContext, CrowdfundingProjectIndex>();
        CreateMap<LogEventContext, UserProjectInfoIndex>();
        CreateMap<LogEventContext, UserRecordIndex>();
        CreateMap<TokenBasicInfo, TokenBasicInfoDto>();
        CreateMap<CrowdfundingProjectIndex, CrowdfundingProjectDto>()
            .ForMember(des => des.BlockHash, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHash))
            .ForMember(des => des.BlockHeight, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHeight))
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId))
            ;
        CreateMap<UserProjectInfoIndex, UserProjectInfoSyncDto>()
            .ForMember(des => des.BlockHash, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHash))
            .ForMember(des => des.BlockHeight, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHeight))
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId))
            ;
        CreateMap<UserRecordIndex, UserRecordDto>()
            .ForMember(des => des.BlockHash, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHash))
            .ForMember(des => des.BlockHeight, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHeight))
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId))
            ;
        CreateMap<LogEventContext, WhitelistIndex>();
        CreateMap<WhitelistReenable, WhitelistIndex>();
        CreateMap<WhitelistDisabled, WhitelistIndex>();
        CreateMap<WhitelistIndex, WhitelistDto>()
            .ForMember(des => des.BlockHash, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHash))
            .ForMember(des => des.BlockHeight, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHeight))
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId))
            ;
        CreateMap<CrowdfundingProjectBase, CrowdfundingProjectBaseDto>()
            .ForMember(des => des.BlockHash, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHash))
            .ForMember(des => des.BlockHeight, opt
                => opt.MapFrom(source => source.Metadata.Block.BlockHeight))
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId))
            ;
        CreateMap<TokenInfoIndex, GetUserTokensDto>()
            .ForMember(des => des.ChainId, opt
                => opt.MapFrom(source => source.Metadata.ChainId));
    }
}