using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IczpNet.AbpCommons.MongoDB;

[ConnectionStringName(AbpCommonsDbProperties.ConnectionStringName)]
public class AbpCommonsMongoDbContext : AbpMongoDbContext, IAbpCommonsMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureAbpCommons();
    }
}
