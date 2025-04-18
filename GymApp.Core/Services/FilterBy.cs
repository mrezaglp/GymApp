
using System.Linq.Expressions;

public class FilterBy
{
    private Expression? _expression;

    public string PropertyName { get; set; }

    public FilterOperation Operation { get; set; }

    public object Value { get; set; }

    public FilterBy()
    {
    }

    public FilterBy(string propertyName, FilterOperation operation, object value)
    {
        PropertyName = propertyName;
        Operation = operation;
        Value = value;
    }

    public bool HasExpression()
    {
        return _expression != null;
    }

    public Expression? Expression(ConstantExpression value)
    {
        return _expression;
    }

    public FilterBy EvaluateExpression(Expression expression)
    {
        _expression = expression;
        return this;
    }

    public Expression Translate<T>(ParameterExpression param)
    {
        return new FilterTranslator().Translate<T>(this, param);
    }
}