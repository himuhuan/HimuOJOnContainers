namespace HimuOJ.Common.DomainSeedWork;

public interface IRepository<T, in TKey>
    where T : IAggregateRoot
    where TKey : IComparable
{
    public IUnitOfWork UnitOfWork { get; }

    public T Add(T entity);

    public void Update(T entity);

    public Task<T> GetAsync(TKey id);
}