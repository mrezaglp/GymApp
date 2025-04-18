
using System;
using System.Linq.Expressions;

namespace Ardalis.Specification;

public class DistinctExpressionInfo<T>
{
    private readonly Lazy<Func<T, object>> _distinctionFunc;

    public Expression<Func<T, object>> Distinction { get; }

    public Func<T, object> DistinctionFunc => _distinctionFunc.Value;

    public DistinctExpressionInfo(Expression<Func<T, object>> distinction)
    {
        if (distinction == null)
        {
            throw new ArgumentNullException("argument distinction could not be null");
        }

        Distinction = distinction;
        _distinctionFunc = new Lazy<Func<T, object>>(Distinction.Compile);
    }
}