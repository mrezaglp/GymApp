
using Ardalis.Specification;
public interface IBaseReadRepository<T> where T : class
{
    IDbContextProvider Provider { get; }

    IDbContext Db { get; }

    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default(CancellationToken)) where TId : notnull;

    [Obsolete]
    Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    [Obsolete]
    Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<List<T>> ListAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<long> SumAsync(ISpecification<T, long> specification, CancellationToken cancellationToken = default(CancellationToken));

    IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification);

    IAsyncEnumerable<TResult> AsAsyncEnumerable<TResult>(ISpecification<T, TResult> specification);
}