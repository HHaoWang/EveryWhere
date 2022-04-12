using Newtonsoft.Json;
using System.Globalization;

namespace EveryWhere.Database.JsonConverter;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string Format = "HH:mm:ss";

    public override TimeOnly ReadJson(JsonReader reader,
        Type objectType,
        TimeOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer) =>
        TimeOnly.ParseExact((string)(reader.Value ?? ""), Format, CultureInfo.InvariantCulture);

    public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer) =>
        writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
}