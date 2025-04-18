
using System.Linq.Expressions;

public class SearchFilterTranslator
{
    public Expression Translate<T>(ISearchFilter sf, ParameterExpression param)
    {
        if (sf.Filters == null || sf.Filters.Count <= 0)
        {
            if (sf.Filter == null)
            {
                return null;
            }

            sf.AddFilter(sf.Filter);
            sf.Filter = null;
        }

        if (sf.Filters.Count >= 1 && sf.Filter != null)
        {
            sf.AddFilter(sf.Filter);
            sf.Filter = null;
        }

        Expression expression = null;
        expression = ((sf.Filters[0].Filter == null) ? Translate<T>(sf.Filters[0], param) : sf.Filters[0].Filter.Translate<T>(param));
        for (int i = 1; i < sf.Filters.Count; i++)
        {
            ISearchFilter searchFilter = sf.Filters[i];
            Expression expression2 = null;
            expression2 = ((searchFilter.Filter == null) ? Translate<T>(searchFilter, param) : searchFilter.Filter.Translate<T>(param));
            if (expression == null)
            {
                expression = expression2;
                continue;
            }

            switch (searchFilter.Connector)
            {
                case FilterConnector.And:
                    if (expression2 == null)
                    {
                        return expression;
                    }

                    expression = Expression.AndAlso(expression, expression2);
                    break;
                case FilterConnector.Or:
                    if (expression2 == null)
                    {
                        return expression;
                    }

                    expression = Expression.OrElse(expression, expression2);
                    break;
            }
        }

        return expression;
    }
}