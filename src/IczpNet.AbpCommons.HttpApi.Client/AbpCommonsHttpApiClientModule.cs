using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpCommonsApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AbpCommonsHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AbpCommonsApplicationContractsModule).Assembly,
            AbpCommonsRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpCommonsHttpApiClientModule>();
        });

    }
}
