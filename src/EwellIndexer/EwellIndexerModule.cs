using AeFinder.Sdk.Processor;
using EwellIndexer.GraphQL;
using EwellIndexer.Processors;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EwellIndexer;

public class EwellIndexerModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<EwellIndexerModule>(); });
        context.Services.AddSingleton<ISchema, AeIndexerSchema>();
        
        context.Services.AddTransient<ILogEventProcessor, TokenCreatedLogEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, TokenBurnedEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, TokenIssuedEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, TokenTransferredLogEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, TokenCrossChainReceivedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, TokenCrossChainTransferredProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProxyAccountCreatedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProxyAccountManagementAddressAddedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProxyAccountManagementAddressRemovedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProxyAccountManagementAddressResetProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProjectRegisteredProcessor>();
        context.Services.AddTransient<ILogEventProcessor, InvestedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, DisInvestedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, RefundedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ClaimedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, AdditionalInfoUpdatedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, LiquidatedDamageProportionUpdatedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ClaimDamageLogEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, NewWhitelistIdSetLogEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, PeriodUpdatedProcessor>();
        context.Services.AddTransient<ILogEventProcessor, ProjectCanceledProcessor>();
        context.Services.AddTransient<ILogEventProcessor, WithdrawnProcessor>();
        context.Services.AddTransient<ILogEventProcessor, WhitelistReenableLogEventProcessor>();
        context.Services.AddTransient<ILogEventProcessor, WhitelistDisabledLogEventProcessor>();
    }
}