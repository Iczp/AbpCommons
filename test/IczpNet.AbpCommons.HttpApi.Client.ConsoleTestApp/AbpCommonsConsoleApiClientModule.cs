using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCommonsHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class AbpCommonsConsoleApiClientModule : AbpModule
{

}
