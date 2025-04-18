
using System.Linq.Expressions;

public class GroupByExpressionInfo<T>
{
    public Expression<Func<T, object>> GroupBy { get; private set; }

    public Expression<Func<IGrouping<object, T>, T>> GroupBySelector { get; private set; }

    public GroupByExpressionInfo(Expression<Func<T, object>> groupBy, Expression<Func<IGrouping<object, T>, T>> selector = null)
    {
        if (groupBy == null)
        {
            throw new ArgumentNullException("argument groupBy could not be null");
        }

        GroupBy = groupBy;
        GroupBySelector = selector;
    }
}