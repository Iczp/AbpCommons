using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCommonsDomainSharedModule)
)]
public class AbpCommonsDomainModule : AbpModule
{
    //public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    //{
    //    //var app = context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;

    //    //app.UseStaticJsonSerializer();

    //    //app.UseStaticAutoMapper();

    //    await base.OnApplicationInitializationAsync(context);
    //}
}
