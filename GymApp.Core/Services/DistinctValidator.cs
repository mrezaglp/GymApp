
namespace Ardalis.Specification;

public class DistinctValidator : IValidator
{
    public static DistinctValidator Instance { get; } = new DistinctValidator();


    private DistinctValidator()
    {
    }

    public bool IsValid<T>(T entity, ISpecification<T> specification)
    {
        foreach (DistinctExpressionInfo<T> distinctExpression in (specification as IBaseSpecification<T>).DistinctExpressions)
        {
            Type type = distinctExpression.DistinctionFunc(entity)?.GetType();
            if (((object)type != null && !type.IsPrimitive) || ((object)type != null && !type.IsEnum) || ((object)type != null && type.IsValueType))
            {
                return false;
            }
        }

        return true;
    }
}