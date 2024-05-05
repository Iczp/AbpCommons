namespace IczpNet.AbpCommons.DataFilters;

public interface IDestination<TKey, T> : IDestination<TKey>, IDestinationObject<T>
{
}

public interface IDestination<TKey>
{
    TKey DestinationId { get; }
}
