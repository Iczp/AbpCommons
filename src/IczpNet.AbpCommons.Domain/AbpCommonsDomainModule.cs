using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using IczpNet.AbpCommons.Extensions;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCommonsDomainSharedModule)
)]
public class AbpCommonsDomainModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;

        app.UseStaticJsonSerializer();

        app.UseStaticAutoMapper();

        await base.OnApplicationInitializationAsync(context);
    }
}
