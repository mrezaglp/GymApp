
using Ardalis.Specification;
using GymApp.Core.Interfaces;

public abstract class BaseWriteRepository<T> : BaseReadRepository<T>, IRepository<T>, IWriteRepository<T>, IBaseReadRepository<T> where T : class, IAggregateRoot
{
    protected BaseWriteRepository(IDbContext dbContext, ISpecificationEvaluator specificationEvaluator)
        : base(dbContext, specificationEvaluator)
    {
    }

    protected abstract Task addRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task updateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    protected abstract Task deleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        await AddRangeAsync(new _003C_003Ez__ReadOnlyArray<T>(new T[1] { entity }), cancellationToken);
        return entity;
    }

    public override Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default(CancellationToken))
    {
        TId id2 = id;
        return FirstOrDefaultAsync(BaseSearchSpec<T>.Default((T x) => id2.Equals(x.GetPrimaryKey())), cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        await addRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        await UpdateRangeAsync(new _003C_003Ez__ReadOnlyArray<T>(new T[1] { entity }), cancellationToken);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        await updateRangeAsync(entities, cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
    {
        await deleteRangeAsync(new _003C_003Ez__ReadOnlyArray<T>(new T[1] { entity }), cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
    {
        await deleteRangeAsync(entities, cancellationToken);
    }

    public virtual Task<BulkOperation<T>> BulkAsync(IBulkSpecification<T> specification, CancellationToken cancellationToken = default(CancellationToken))
    {
        BulkOperation<T> bulkOperation = specification.GetBulkOpveration(ApplySpecification(specification));
        base.Db.BulkAsync(bulkOperation, cancellationToken);
        return ValueTask.FromResult(bulkOperation).AsTask();
    }

    public async Task DeleteAsync(T entity, bool softDelete = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        await DeleteRangeAsync(new _003C_003Ez__ReadOnlyArray<T>(new T[1] { entity }), softDelete, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool softDelete = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!softDelete)
        {
            await DeleteRangeAsync(entities, cancellationToken);
        }
        else
        {
            foreach (T entity in entities)
            {
                entity.GetRootEntity().Modified();
                entity.GetRootEntity().Delete();
            }

            await UpdateRangeAsync(entities, cancellationToken);
        }

        await ValueTask.CompletedTask.AsTask();
    }

    public async Task UpdateVisibilityAsync(T entity, bool visible = false, CancellationToken cancellationToken = default(CancellationToken))
    {
        bool isVisibled = entity.GetRootEntity().IsVisibled;
        if (visible)
        {
            entity.GetRootEntity().Visible();
        }
        else
        {
            entity.GetRootEntity().InVisible();
        }

        if (isVisibled != entity.GetRootEntity().IsVisibled)
        {
            entity.GetRootEntity().Modified();
            await UpdateAsync(entity, cancellationToken);
        }

        await ValueTask.CompletedTask.AsTask();
    }
}