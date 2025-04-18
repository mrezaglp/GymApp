
using GymApp.Core.Interfaces;

public interface IWriteRepository<T> : IBaseReadRepository<T> where T : class, IAggregateRoot
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteAsync(T entity, bool softDelete = true, CancellationToken cancellationToken = default(CancellationToken));

    Task DeleteRangeAsync(IEnumerable<T> entities, bool softDelete = true, CancellationToken cancellationToken = default(CancellationToken));

    Task UpdateVisibilityAsync(T entity, bool visible = false, CancellationToken cancellationToken = default(CancellationToken));

    Task<BulkOperation<T>> BulkAsync(IBulkSpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));
}