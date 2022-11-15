using IczpNet.AbpCommons.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpCommonsEntityFrameworkCoreTestModule)
    )]
public class AbpCommonsDomainTestModule : AbpModule
{

}
