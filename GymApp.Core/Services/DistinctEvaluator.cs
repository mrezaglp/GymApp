
namespace Ardalis.Specification;

public class DistinctEvaluator : IEvaluator, IInMemoryEvaluator
{
    public bool IsCriteriaEvaluator => true;

    public static DistinctEvaluator Instance { get; } = new DistinctEvaluator();


    private DistinctEvaluator()
    {
    }

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification)
    {
        if (specification is IBaseSpecification<T> baseSpecification)
        {
            foreach (DistinctExpressionInfo<T> distinctExpression in baseSpecification.DistinctExpressions)
            {
                query = query.AsQueryable().DistinctBy(distinctExpression.Distinction).AsEnumerable();
            }
        }

        return query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification is IBaseSpecification<T> baseSpecification)
        {
            foreach (DistinctExpressionInfo<T> distinctExpression in baseSpecification.DistinctExpressions)
            {
                query = from x in query.GroupBy(distinctExpression.Distinction)
                        select x.First();
            }
        }

        return query;
    }
}