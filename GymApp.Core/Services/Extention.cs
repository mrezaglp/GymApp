
using Newtonsoft.Json.Linq;

public static class Extention
{
    public static string TryParse(string value)
    {
        try
        {
            return JToken.Parse(value).ToString();
        }
        catch (Exception)
        {
        }

        return null;
    }

    public static bool TryFromObject(object value, out dynamic result)
    {
        try
        {
            int num = JToken.FromObject(value).First.First.Value<int>();
            if (num == 3 || num != 4)
            {
                result = value.ToString();
            }
            else
            {
                result = Convert.ToInt32(value);
            }

            return true;
        }
        catch (Exception)
        {
            result = null;
        }

        return false;
    }

    public static dynamic ConvertTo(this object source, Type typeTo, bool isNullable = false)
    {
        Type typeTo2 = typeTo;
        object source2 = source;
        string text = (typeTo2.IsEnum ? "Enum" : typeTo2.Name);
        if (isNullable && typeTo2.GenericTypeArguments.Length != 0)
        {
            text = typeTo2.GenericTypeArguments[0].Name;
        }

        switch (text)
        {
            case "Int16":
            case "Int32":
            case "Int64":
            case "Byte":
            case "Enum":
            case "String":
            case "DateTime":
            case "Boolean":
                return getValue();
            default:
                return null;
        }

        dynamic getValue()
        {
            try
            {
                dynamic val = (typeTo2.IsEnum ? Enum.Parse(typeTo2, source2.ToString(), ignoreCase: true) : source2);
                if (val is IConvertible)
                {
                    val = Convert.ChangeType((object?)val, typeTo2);
                }

                if (TryFromObject(source2, out object result))
                {
                    val = result;
                }

                if (isNullable)
                {
                    return Extention.GetNullable(val);
                }

                return val;
            }
            catch (Exception)
            {
                return typeTo2.GetDefaultValue();
            }
        }
    }

    public static dynamic CreateGenericList(this object data, dynamic defulatValue, bool isNullable = false)
    {
        JArray jArray = JArray.FromObject(data);
        dynamic val = null;
        val = (isNullable ? Extention.GetNullableList(defulatValue) : Extention.GetList(defulatValue));
        foreach (JToken item in jArray)
        {
            dynamic val2 = item.ToObject<object>();
            dynamic val3 = Extention.ConvertTo(val2, defulatValue.GetType(), isNullable);
            val.Add(val3);
        }

        return val;
    }

    public static dynamic GetList<T>(this T value)
    {
        return Activator.CreateInstance(typeof(List<>).MakeGenericType(value.GetType()));
    }

    public static dynamic GetNullableList<T>(this T value) where T : struct
    {
        Type type = Type.GetType($"System.Nullable`1[{value.GetType()}]");
        return Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
    }

    public static T? GetNullable<T>(this T s) where T : struct
    {
        return s;
    }

    public static bool IsList(this object source)
    {
        if (source == null)
        {
            return false;
        }

        return JToken.FromObject(source) is JArray;
    }

    public static object GetDefaultValue(this Type t)
    {
        if (t.IsValueType)
        {
            if ((object)Nullable.GetUnderlyingType(t) != null)
            {
                t = Nullable.GetUnderlyingType(t);
            }

            return Activator.CreateInstance(t);
        }

        return null;
    }
}