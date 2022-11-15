using Volo.Abp;
using Volo.Abp.MongoDB;

namespace IczpNet.AbpCommons.MongoDB;

public static class AbpCommonsMongoDbContextExtensions
{
    public static void ConfigureAbpCommons(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
