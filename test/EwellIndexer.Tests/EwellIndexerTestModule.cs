using AeFinder.App.TestBase;
using EwellIndexer.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace EwellIndexer;

[DependsOn(
    typeof(AeFinderAppTestBaseModule),
    typeof(EwellIndexerModule))]
public class EwellIndexerTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AeFinderAppEntityOptions>(options => { options.AddTypes<EwellIndexerModule>(); });
        
        context.Services.AddSingleton<TokenCreatedLogEventProcessor>();
        context.Services.AddSingleton<TokenBurnedEventProcessor>();
        context.Services.AddSingleton<TokenIssuedEventProcessor>();
        context.Services.AddSingleton<TokenTransferredLogEventProcessor>();
        context.Services.AddSingleton<TokenCrossChainReceivedProcessor>();
        context.Services.AddSingleton<TokenCrossChainTransferredProcessor>();
        context.Services.AddSingleton<ProxyAccountCreatedProcessor>();
        context.Services.AddSingleton<ProxyAccountManagementAddressAddedProcessor>();
        context.Services.AddSingleton<ProxyAccountManagementAddressRemovedProcessor>();
        context.Services.AddSingleton<ProxyAccountManagementAddressResetProcessor>();
        context.Services.AddSingleton<ProjectRegisteredProcessor>();
        context.Services.AddSingleton<InvestedProcessor>();
        context.Services.AddSingleton<DisInvestedProcessor>();
        context.Services.AddSingleton<RefundedProcessor>();
        context.Services.AddSingleton<ClaimedProcessor>();
        context.Services.AddSingleton<AdditionalInfoUpdatedProcessor>();
        context.Services.AddSingleton<LiquidatedDamageProportionUpdatedProcessor>();
        context.Services.AddSingleton<ClaimDamageLogEventProcessor>();
        context.Services.AddSingleton<NewWhitelistIdSetLogEventProcessor>();
        context.Services.AddSingleton<PeriodUpdatedProcessor>();
        context.Services.AddSingleton<ProjectCanceledProcessor>();
        context.Services.AddSingleton<WithdrawnProcessor>();
        context.Services.AddSingleton<WhitelistReenableLogEventProcessor>();
        context.Services.AddSingleton<WhitelistDisabledLogEventProcessor>();
    }
}