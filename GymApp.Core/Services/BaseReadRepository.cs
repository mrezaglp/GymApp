
using Ardalis.Specification;
using GymApp.Core.Interfaces;

public abstract class BaseReadRepository<T> : BaseRepository<T>, IReadRepository<T>, IBaseReadRepository<T> where T : class, IMuteEntity
{
    public string DbProviderName => base.Db.DbProviderName;

    protected BaseReadRepository(IDbContext dbContext, ISpecificationEvaluator specificationEvaluator)
        : base(dbContext, specificationEvaluator)
    {
    }

    protected abstract Task<T?> firstOrDefaultAsync(IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<TResult> firstOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<T?> singleOrDefaultAsync(IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<TResult?> singleOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<List<T>> toListAsync(CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<List<TResult>> toListAsync<TResult>(IQueryable<TResult> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<int> countAsync(IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task<bool> anyAsync(IQueryable<T> query, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract IQueryable<T> getAsQueryable();

    [Obsolete("Please use FirstOrDefaultAsync instead")]
    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default(CancellationToken)) where TId : notnull
    {
        throw new NotImplementedException();
    }

    [Obsolete("Please use FirstOrDefaultAsync instead")]
    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await FirstOrDefaultAsync(specification, cancellationToken);
    }

    [Obsolete("Please use FirstOrDefaultAsync instead")]
    public virtual async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await FirstOrDefaultAsync(specification, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await firstOrDefaultAsync(ApplySpecification(specification), cancellationToken);
    }

    public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await firstOrDefaultAsync(ApplySpecification(specification), cancellationToken);
    }

    public virtual async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await singleOrDefaultAsync(ApplySpecification(specification), cancellationToken);
    }

    public virtual async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await singleOrDefaultAsync(ApplySpecification(specification), cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await toListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        List<T> list = await toListAsync(ApplySpecification(specification), cancellationToken);
        return (specification.PostProcessingAction == null) ? list : specification.PostProcessingAction(list).ToList();
    }

    public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        List<TResult> list = await toListAsync(ApplySpecification(specification), cancellationToken);
        return (specification.PostProcessingAction == null) ? list : specification.PostProcessingAction(list).ToList();
    }

    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await countAsync(ApplySpecification(specification, evaluateCriteriaOnly: true), cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await CountAsync(BaseSearchSpec<T>.Default(), cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await anyAsync(ApplySpecification(specification, evaluateCriteriaOnly: true), cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await AnyAsync(BaseSearchSpec<T>.Default(), cancellationToken);
    }

    public virtual IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification)
    {
        return (IAsyncEnumerable<T>)ApplySpecification(specification);
    }

    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
    {
        return specificationEvaluator.GetQuery(getAsQueryable(), specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
    {
        return specificationEvaluator.GetQuery(getAsQueryable(), specification);
    }

    public Task<long> SumAsync(ISpecification<T, long> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public virtual IAsyncEnumerable<TResult> AsAsyncEnumerable<TResult>(ISpecification<T, TResult> specification)
    {
        return (IAsyncEnumerable<TResult>)ApplySpecification(specification);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}