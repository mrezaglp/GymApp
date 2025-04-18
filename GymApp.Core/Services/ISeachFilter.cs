
using System.Linq.Expressions;

public interface ISearchFilter : IFingerprintable
{
    FilterBy? Filter { get; set; }

    FilterConnector Connector { get; set; }

    List<ISearchFilter> Filters { get; set; }

    void AddFilter(ISearchFilter filter);

    void AddFilter(FilterBy filter);

    void OrFilter(ISearchFilter filter);

    void OrFilter(FilterBy filter);

    Expression<Func<T, bool>> Translate<T>();

    Expression<Func<TTo, bool>> Convert<TFrom, TTo>();

    Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst);

    ISearchFilter Rename(ISearchFilter sf);
}