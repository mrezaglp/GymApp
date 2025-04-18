
using System;
using System.Linq;

namespace Ardalis.Specification;

public class RawQueryExpressionInfo<T>
{
    public string RawQuery { get; }

    public string[] Args { get; }

    public Lazy<Func<string, string[], IQueryable<T>>> RawQueryFunc { get; set; }

    public RawQueryExpressionInfo(string rawQuery)
    {
        if (rawQuery == null)
        {
            throw new ArgumentNullException("argument rawQuery could not be null");
        }

        RawQuery = rawQuery;
        RawQueryFunc = new Lazy<Func<string, string[], IQueryable<T>>>();
    }

    public RawQueryExpressionInfo(string rawQuery, params string[] argus)
    {
        if (rawQuery == null)
        {
            throw new ArgumentNullException("argument rawQuery could not be null");
        }

        RawQuery = rawQuery;
        Args = argus;
        RawQueryFunc = new Lazy<Func<string, string[], IQueryable<T>>>();
    }
}