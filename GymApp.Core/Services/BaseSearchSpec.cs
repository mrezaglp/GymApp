
using System.Linq.Expressions;
using Ardalis.Specification;
using GymApp.Core.Interfaces;

public abstract class BaseSearchSpec<T> : Specification<T>, IBaseSpecification<T>, ISpecification<T> where T : IMuteEntity
{
    private class InnerBaseSearchSpec<T> : BaseSearchSpec<T> where T : IMuteEntity
    {
        public InnerBaseSearchSpec(Expression<Func<T, bool>>? criteria = null)
        {
            if (criteria != null)
            {
                Query.Where(criteria);
            }
        }
    }

    private bool _BulkDelete;

    public IEnumerable<DistinctExpressionInfo<T>> DistinctExpressions { get; } = new List<DistinctExpressionInfo<T>>();


    public IEnumerable<GroupByExpressionInfo<T>> GroupByExpressions { get; } = new List<GroupByExpressionInfo<T>>();


    public IEnumerable<RawQueryExpressionInfo<T>> RawQueries { get; } = new List<RawQueryExpressionInfo<T>>();


    public IEnumerable<BulkUpdate<T, object>> BulkUpdates { get; } = new List<BulkUpdate<T, object>>();


    public bool BulkDelete => _BulkDelete;

    public bool IsIncrementTake { get; private set; }

    public long? Cursor { get; private set; }

    public CursorDirectionEnum? Direction { get; protected set; } = CursorDirectionEnum.Before;


    public BaseSearchSpec()
        : base((IInMemorySpecificationEvaluator)new InMemorySpecificationEvaluator(new IInMemoryEvaluator[7]
        {
            WhereEvaluator.Instance,
            SearchEvaluator.Instance,
            OrderEvaluator.Instance,
            DistinctEvaluator.Instance,
            PaginationEvaluator.Instance,
            GroupByEvaluator.Instance,
            RawQueryEvaluator.Instance
        }), (ISpecificationValidator)new SpecificationValidator(new IValidator[3]
        {
            WhereValidator.Instance,
            SearchValidator.Instance,
            DistinctValidator.Instance
        }))
    {
        Query.Where((T item) => !((ISoftDelete)item).IsDeleted);
    }

    public BaseSearchSpec(IInMemoryEvaluator[] inMemoryEvaluators, IValidator[] validators)
        : base((IInMemorySpecificationEvaluator)new InMemorySpecificationEvaluator(inMemoryEvaluators), (ISpecificationValidator)new SpecificationValidator(validators))
    {
        Query.Where((T item) => !((ISoftDelete)item).IsDeleted);
    }

    public static BaseSearchSpec<T> Default(Expression<Func<T, bool>>? criteria = null)
    {
        return Default<T>(criteria);
    }

    protected internal static BaseSearchSpec<T> DefaultBuidler(Func<ISpecificationBuilder<T>, BaseSearchSpec<T>>? operation)
    {
        return operation(new InnerBaseSearchSpec<T>().Query);
    }

    private static BaseSearchSpec<TEntity> Default<TEntity>(Expression<Func<TEntity, bool>>? criteria = null) where TEntity : IMuteEntity
    {
        return new InnerBaseSearchSpec<TEntity>(criteria);
    }

    public void IncrementTake(int take)
    {
        IsIncrementTake = true;
        Query.Take(take + 1);
    }

    protected void handleCursor(ISearchFilter filter)
    {
        if (filter is ICursorTermFilter cursorTermFilter)
        {
            Cursor = cursorTermFilter.Cursor;
        }
    }

    protected void handleCursor(ITermFilter filter)
    {
        if (filter is ICursorTermFilter cursorTermFilter)
        {
            Cursor = cursorTermFilter.Cursor;
        }
    }
}