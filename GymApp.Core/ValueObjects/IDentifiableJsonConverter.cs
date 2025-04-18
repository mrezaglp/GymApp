using Newtonsoft.Json;

namespace GymApp.ValueObjects;

public class IDentifiableJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        return new IDentifiable(reader.Value?.ToString() ?? IDentifiable.Empty);
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}