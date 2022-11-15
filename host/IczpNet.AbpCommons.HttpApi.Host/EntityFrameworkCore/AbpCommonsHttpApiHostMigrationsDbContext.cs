using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.AbpCommons.EntityFrameworkCore;

public class AbpCommonsHttpApiHostMigrationsDbContext : AbpDbContext<AbpCommonsHttpApiHostMigrationsDbContext>
{
    public AbpCommonsHttpApiHostMigrationsDbContext(DbContextOptions<AbpCommonsHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureAbpCommons();
    }
}
