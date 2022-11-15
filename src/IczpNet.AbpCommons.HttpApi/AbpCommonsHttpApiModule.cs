using Localization.Resources.AbpUi;
using IczpNet.AbpCommons.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpCommonsApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class AbpCommonsHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpCommonsHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AbpCommonsResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
