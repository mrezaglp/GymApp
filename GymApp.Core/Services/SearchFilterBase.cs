
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
public class SearchFilterBase : ISearchFilter, IFingerprintable
{
    public FilterBy? Filter { get; set; }

    public FilterConnector Connector { get; set; }

    public List<ISearchFilter> Filters { get; set; } = new List<ISearchFilter>();


    public void AddFilter(ISearchFilter filter)
    {
        filter.Connector = FilterConnector.And;
        Filters.Add(filter);
    }

    public void AddFilter(FilterBy filter)
    {
        Filters.Add(new SearchFilterBase
        {
            Filter = filter,
            Connector = FilterConnector.And
        });
    }

    public void OrFilter(ISearchFilter filter)
    {
        filter.Connector = FilterConnector.Or;
        Filters.Add(filter);
    }

    public void OrFilter(FilterBy filter)
    {
        Filters.Add(new SearchFilterBase
        {
            Filter = filter,
            Connector = FilterConnector.Or
        });
    }

    public virtual Expression<Func<T, bool>> Translate<T>()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name.ToLower());
        Expression expression = new SearchFilterTranslator().Translate<T>(this, parameterExpression);
        if (expression == null)
        {
            return null;
        }

        return Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[1] { parameterExpression });
    }

    public Expression<Func<TTo, bool>> Convert<TFrom, TTo>()
    {
        Expression<Func<TFrom, bool>> expression = Translate<TFrom>();
        Dictionary<Expression, Expression> dictionary = new Dictionary<Expression, Expression>();
        ParameterExpression parameterExpression = expression.Parameters[0];
        ParameterExpression parameterExpression2 = Expression.Parameter(typeof(TTo), parameterExpression.Name);
        dictionary.Add(parameterExpression, parameterExpression2);
        return Expression.Lambda<Func<TTo, bool>>(ConvertNode(expression.Body, dictionary), new ParameterExpression[1] { parameterExpression2 });
    }

    public Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst)
    {
        if (node == null)
        {
            return null;
        }

        if (subst.ContainsKey(node))
        {
            return subst[node];
        }

        switch (node.NodeType)
        {
            case ExpressionType.Constant:
                return node;
            case ExpressionType.MemberAccess:
                {
                    MemberExpression memberExpression = (MemberExpression)node;
                    Expression expression = ConvertNode(memberExpression.Expression, subst);
                    return Expression.MakeMemberAccess(expression, expression.Type.GetMember(memberExpression.Member.Name).Single());
                }
            case ExpressionType.Equal:
                {
                    BinaryExpression binaryExpression = (BinaryExpression)node;
                    return Expression.MakeBinary(binaryExpression.NodeType, ConvertNode(binaryExpression.Left, subst), ConvertNode(binaryExpression.Right, subst), binaryExpression.IsLiftedToNull, binaryExpression.Method);
                }
            default:
                throw new NotSupportedException(node.NodeType.ToString());
        }
    }

    public ISearchFilter Rename(ISearchFilter sf)
    {
        if (sf.Filters.Count == 0 && sf.Filter != null)
        {
            sf.AddFilter(sf.Filter);
        }

        for (int i = 0; i < sf.Filters.Count; i++)
        {
            ISearchFilter searchFilter = sf.Filters[i];
            if (searchFilter.Filter == null)
            {
                sf.Filters[i] = Rename(searchFilter);
            }
            else
            {
                searchFilter.Filter.PropertyName = searchFilter.Filter.PropertyName.Replace(".", "_");
            }
        }

        return sf;
    }
}