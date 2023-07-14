using System;

namespace IczpNet.AbpCommons.DataFilters
{
    public interface IAppUserId : IAppUserId<Guid>
    {
    }

    public interface IAppUserId<TKey> where TKey : struct
    {
        TKey? AppUserId { get; }
    }
}
