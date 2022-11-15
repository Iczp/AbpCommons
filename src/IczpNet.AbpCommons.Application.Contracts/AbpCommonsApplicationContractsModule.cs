using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpCommonsDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class AbpCommonsApplicationContractsModule : AbpModule
{

}
