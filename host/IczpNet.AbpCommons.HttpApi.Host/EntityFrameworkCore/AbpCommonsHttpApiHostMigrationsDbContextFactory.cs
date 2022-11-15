using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IczpNet.AbpCommons.EntityFrameworkCore;

public class AbpCommonsHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<AbpCommonsHttpApiHostMigrationsDbContext>
{
    public AbpCommonsHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<AbpCommonsHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("AbpCommons"));

        return new AbpCommonsHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
