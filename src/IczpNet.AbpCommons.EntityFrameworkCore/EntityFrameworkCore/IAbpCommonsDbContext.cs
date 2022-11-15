using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.AbpCommons.EntityFrameworkCore;

[ConnectionStringName(AbpCommonsDbProperties.ConnectionStringName)]
public interface IAbpCommonsDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
