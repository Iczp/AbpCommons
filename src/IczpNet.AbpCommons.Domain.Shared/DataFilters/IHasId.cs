namespace IczpNet.AbpCommons.DataFilters
{
    public interface IHasId<TKey>
    {
        TKey Id { get; }
    }
}
