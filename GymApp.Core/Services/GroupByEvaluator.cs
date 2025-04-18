
using Ardalis.Specification;

public class GroupByEvaluator : IEvaluator, IInMemoryEvaluator
{
    public bool IsCriteriaEvaluator => true;

    public static GroupByEvaluator Instance { get; } = new GroupByEvaluator();


    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is IBaseSpecification<T> baseSpecification)
        {
            foreach (GroupByExpressionInfo<T> groupByExpression in baseSpecification.GroupByExpressions)
            {
                IQueryable<IGrouping<object, T>> source = query.GroupBy(groupByExpression.GroupBy);
                query = ((groupByExpression.GroupBySelector == null) ? source.SelectMany((IGrouping<object, T> x) => x) : source.Select(groupByExpression.GroupBySelector));
            }
        }

        return query;
    }

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
    {
        return query;
    }
}