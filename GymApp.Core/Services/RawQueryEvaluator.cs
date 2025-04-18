
using Ardalis.Specification;

public class RawQueryEvaluator : IEvaluator, IInMemoryEvaluator
{
    public static RawQueryEvaluator Instance = new RawQueryEvaluator();

    public bool IsCriteriaEvaluator => true;

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
    {
        return query.AsEnumerable();
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is IBaseSpecification<T> baseSpecification)
        {
            RawQueryExpressionInfo<T> rawQueryExpressionInfo = baseSpecification.RawQueries.FirstOrDefault();
            if (rawQueryExpressionInfo != null && rawQueryExpressionInfo.RawQueryFunc.IsValueCreated)
            {
                query = rawQueryExpressionInfo.RawQueryFunc.Value(rawQueryExpressionInfo.RawQuery, rawQueryExpressionInfo.Args);
            }
        }

        return query;
    }
}