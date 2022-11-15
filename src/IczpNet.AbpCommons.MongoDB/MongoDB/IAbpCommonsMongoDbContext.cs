using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IczpNet.AbpCommons.MongoDB;

[ConnectionStringName(AbpCommonsDbProperties.ConnectionStringName)]
public interface IAbpCommonsMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
