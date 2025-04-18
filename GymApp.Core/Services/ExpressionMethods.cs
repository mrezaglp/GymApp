
using System.Linq.Expressions;
using System.Reflection;

public static class ExpressionMethods
{
    public static MethodInfo GetMethodByName(this MemberExpression m, string methodName)
    {
        string methodName2 = methodName;
        return (from x in m.Type.GetMethods()
                where x.Name.Equals(methodName2)
                select x).FirstOrDefault();
    }
}