using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpCommonsApplicationModule),
    typeof(AbpCommonsDomainTestModule)
    )]
public class AbpCommonsApplicationTestModule : AbpModule
{

}
