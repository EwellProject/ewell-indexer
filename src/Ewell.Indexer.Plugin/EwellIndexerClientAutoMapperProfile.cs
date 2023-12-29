using AElfIndexer.Client.Handlers;
using AutoMapper;
using Ewell.Indexer.Plugin.Entities;

namespace Ewell.Indexer.Plugin;

public class EwellIndexerClientAutoMapperProfile : Profile
{
    public EwellIndexerClientAutoMapperProfile()
    {
        CreateMap<LogEventContext, CrowdfundingProjectIndex>();
        CreateMap<LogEventContext, UserProjectInfoIndex>();
        CreateMap<LogEventContext, UserRecordIndex>();
    }
}