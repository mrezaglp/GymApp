
using Ardalis.Specification;

public interface IBaseSpecification<T> : ISpecification<T>
{
    IEnumerable<DistinctExpressionInfo<T>> DistinctExpressions { get; }

    IEnumerable<GroupByExpressionInfo<T>> GroupByExpressions { get; }

    IEnumerable<RawQueryExpressionInfo<T>> RawQueries { get; }

    IEnumerable<BulkUpdate<T, object>> BulkUpdates { get; }

    bool BulkDelete { get; }

    bool IsIncrementTake { get; }

    long? Cursor { get; }

    CursorDirectionEnum? Direction { get; }

    void IncrementTake(int take);
}