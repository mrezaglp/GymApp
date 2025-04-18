
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

public class FilterTranslator
{
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");

    private static readonly MethodInfo ContainsMethod2 = (from x in typeof(string).GetMethods()
                                                          where x.Name.Equals("Contains")
                                                          select x).First();

    private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[1] { typeof(string) });

    private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[1] { typeof(string) });

    private static readonly MethodInfo AnyMethod = typeof(Enumerable).GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.Public).First((MethodInfo m) => m.Name == "Any" && m.GetParameters().Count() == 2);

    public Expression Translate<T>(FilterBy filter, ParameterExpression param)
    {
        return GetExpression<T>(filter, param);
    }

    private MemberExpression GetProperty(Expression param, string propertyName)
    {
        string[] array = propertyName.Split('.');
        MemberExpression memberExpression = Expression.Property(param, array[0]);
        if (array.Length > 1 && !memberExpression.Type.GetTypeInfo().Namespace.Contains("System.Collections"))
        {
            propertyName = propertyName.Replace(array[0] + ".", "");
            return GetProperty(memberExpression, propertyName);
        }

        memberExpression.Type.GetTypeInfo().Namespace.Contains("System.Collections");
        return memberExpression;
    }

    private string PrepareFilter(MemberExpression param, string PropertyName)
    {
        List<string> list = PropertyName.Split('.').ToList();
        string text = string.Empty;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != param.Type.GetTypeInfo().GetGenericArguments()[0].Name)
            {
                text = text + list[i] + ".";
                continue;
            }

            text = text + list[i] + ".";
            break;
        }

        PropertyName = PropertyName.Replace(text, "");
        return PropertyName;
    }

    public Type GetEnumType(string enumName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type type = assemblies[i].GetType(enumName);
            if (!(type == null) && type.IsEnum)
            {
                return type;
            }
        }

        return null;
    }

    private Expression GetExpression<T>(FilterBy filter, ParameterExpression param)
    {
        Expression result = null;
        ConstantExpression constantExpression = null;
        MemberExpression property = GetProperty(param, filter.PropertyName);
        if (property.Type.GetTypeInfo().Namespace.Contains("System.Collections") && filter.Operation != FilterOperation.Any && filter.Operation != FilterOperation.NotAny)
        {
            ParameterExpression parameterExpression = Expression.Parameter(property.Type.GetTypeInfo().GetGenericArguments()[0], property.ToString().Replace('.', '_') + "_" + property.Type.GetTypeInfo().GetGenericArguments()[0].Name);
            filter.PropertyName = PrepareFilter(property, filter.PropertyName);
            MemberExpression property2 = GetProperty(parameterExpression, filter.PropertyName);
            if (filter.PropertyName.Split('.').ToList().Count <= 2)
            {
                Type conversionType = Nullable.GetUnderlyingType(Type.GetType(property2.Type.FullName)) ?? Type.GetType(property2.Type.FullName);
                constantExpression = Expression.Constant((filter.Value == null) ? null : Convert.ChangeType(filter.Value, conversionType));
            }

            Expression expression = GetExpression<T>(filter, parameterExpression);
            LambdaExpression predicate = Expression.Lambda(typeof(Func<,>).MakeGenericType(property.Type.GetTypeInfo().GetGenericArguments()[0], typeof(bool)), expression, parameterExpression);
            return CallAny(property, predicate);
        }

        Type type = Type.GetType(property.Type.FullName) ?? GetEnumType(property.Type.FullName);
        Type type2 = type;
        bool flag = false;
        object obj = null;
        if (flag = (object)Nullable.GetUnderlyingType(type) != null)
        {
            type2 = Nullable.GetUnderlyingType(type);
        }

        if (!filter.Value.IsList())
        {
            obj = filter.Value.ConvertTo(type2, flag);
            if (obj == null)
            {
                obj = type2.GetDefaultValue();
            }
        }
        else
        {
            object defaultValue = type.GetDefaultValue();
            obj = filter.Value.CreateGenericList(defaultValue, flag);
        }

        constantExpression = ((filter.Operation != FilterOperation.IsNull && filter.Operation != FilterOperation.IsNotNull && filter.Operation != FilterOperation.Any && filter.Operation != FilterOperation.NotAny && filter.Operation != FilterOperation.In && filter.Operation != FilterOperation.NotIn) ? Expression.Constant(Convert.ChangeType(obj, type2), type) : ((filter.Operation != FilterOperation.Any && filter.Operation != FilterOperation.NotAny) ? ((filter.Operation != FilterOperation.In && filter.Operation != FilterOperation.NotIn) ? Expression.Constant(null) : Expression.Constant(obj, obj.GetType())) : Expression.Constant(true)));
        MethodInfo method = (from m in typeof(List<>).MakeGenericType(type).GetTypeInfo().GetMethods()
                             where m.Name == "Contains"
                             select m).First();
        if (filter.HasExpression())
        {
            filter.Expression(constantExpression);
        }

        switch (filter.Operation)
        {
            case FilterOperation.Equals:
                result = Expression.Equal(property, constantExpression);
                break;
            case FilterOperation.IsNull:
                result = Expression.Equal(property, constantExpression);
                break;
            case FilterOperation.IsNotNull:
                result = Expression.NotEqual(property, constantExpression);
                break;
            case FilterOperation.NotEqual:
                result = Expression.NotEqual(property, constantExpression);
                break;
            case FilterOperation.GreaterThan:
                result = Expression.GreaterThan(property, constantExpression);
                break;
            case FilterOperation.GreaterThanOrEqual:
                result = Expression.GreaterThanOrEqual(property, constantExpression);
                break;
            case FilterOperation.LessThan:
                result = Expression.LessThan(property, constantExpression);
                break;
            case FilterOperation.LessThanOrEqual:
                result = Expression.LessThanOrEqual(property, constantExpression);
                break;
            case FilterOperation.Contains:
                {
                    MethodInfo methodByName3 = property.GetMethodByName("Contains");
                    if (methodByName3 != null)
                    {
                        result = Expression.Call(property, methodByName3, constantExpression);
                    }

                    break;
                }
            case FilterOperation.StartsWith:
                {
                    MethodInfo methodByName2 = property.GetMethodByName("StartsWith");
                    if (methodByName2 != null)
                    {
                        result = Expression.Call(property, methodByName2, constantExpression);
                    }

                    break;
                }
            case FilterOperation.EndsWith:
                {
                    MethodInfo methodByName = property.GetMethodByName("EndsWith");
                    if (methodByName != null)
                    {
                        result = Expression.Call(property, methodByName, constantExpression);
                    }

                    break;
                }
            case FilterOperation.Any:
                {
                    Type[] genericArguments = property.Type.GetTypeInfo().GetGenericArguments();
                    if (filter.Value == null && genericArguments.Length != 0)
                    {
                        result = CallAny(property, Expression.Lambda(constantExpression, Expression.Parameter(genericArguments[0], property.ToString().Replace('.', '_'))));
                    }
                    else if (genericArguments.Length != 0)
                    {
                        ParameterExpression parameterExpression3 = Expression.Parameter(genericArguments[0], property.ToString().Replace('.', '_'));
                        SearchFilter sf2 = JsonConvert.DeserializeObject<SearchFilter>(filter.Value.ToString());
                        LambdaExpression predicate3 = Expression.Lambda(new SearchFilterTranslator().Translate<T>(sf2, parameterExpression3), parameterExpression3);
                        result = CallAny(property, predicate3);
                    }

                    break;
                }
            case FilterOperation.NotAny:
                {
                    if (filter.Value == null)
                    {
                        result = Expression.Not(CallAny(property, Expression.Lambda(constantExpression, Expression.Parameter(property.Type.GetTypeInfo().GetGenericArguments()[0], property.ToString().Replace('.', '_')))));
                        break;
                    }

                    ParameterExpression parameterExpression2 = Expression.Parameter(property.Type.GetTypeInfo().GetGenericArguments()[0], property.ToString().Replace('.', '_'));
                    SearchFilter sf = JsonConvert.DeserializeObject<SearchFilter>(filter.Value.ToString());
                    LambdaExpression predicate2 = Expression.Lambda(new SearchFilterTranslator().Translate<T>(sf, parameterExpression2), parameterExpression2);
                    result = Expression.Not(CallAny(property, predicate2));
                    break;
                }
            case FilterOperation.In:
                result = Expression.Call(constantExpression, method, property);
                break;
            case FilterOperation.NotIn:
                result = Expression.Not(Expression.Call(constantExpression, method, property));
                break;
        }

        return result;
    }

    private bool IsIEnumerable(Type type)
    {
        if (type.GetTypeInfo().IsGenericType)
        {
            return type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        return false;
    }

    private Type GetIEnumerableImpl(Type type)
    {
        if (IsIEnumerable(type))
        {
            return type;
        }

        return type.GetTypeInfo().FindInterfaces((Type m, object? o) => IsIEnumerable(m), null)[0];
    }

    private Expression CallAny(Expression collection, Expression predicate)
    {
        Type type = GetIEnumerableImpl(collection.Type).GetGenericArguments()[0];
        typeof(Func<,>).MakeGenericType(type, typeof(bool));
        Type[] typeArgs = new Type[1] { type };
        int typeArity = typeArgs.Length;
        return Expression.Call((from m in typeof(Enumerable).GetTypeInfo().GetMethods()
                                where m.Name == "Any"
                                where m.GetGenericArguments().Length == typeArity
                                select m.MakeGenericMethod(typeArgs)).Last(), collection, predicate);
    }
}