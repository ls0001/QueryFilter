using System.Text.Json;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor.Json;

#region Constant Node Converter

/// <summary>
/// 常量节点转换器
/// </summary>
public class ConstNodeJsonConverter : JsonConverter<ConstantNode>
{
    public JsonValueConvertOptions ValueConvertOptions { get; set; } = new();

    public override ConstantNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        object? value = null;
        Type? valueType = null;
        foreach ( var prop in root.EnumerateObject() )
        {
            if ( ValueConvertOptions.TryGetTypeMappedName(prop.Name, out _) )
            {
                valueType = ValueConvertOptions.GetTypeWinthName(prop.Name);
                value = ConvertJsonElement(prop.Value, valueType, options);
                break;
            }
            else if ( string.Equals(prop.Name, "type", StringComparison.OrdinalIgnoreCase) )
            {
                valueType = ValueConvertOptions.GetTypeWinthName(prop.Value.GetString() ?? "null");
                if ( root.TryGetProperty("value", out var valueProp)
                || root.TryGetProperty("val", out valueProp) )
                    value = ConvertJsonElement(valueProp, valueType, options);
                break;
            }
        }

        if ( valueType == null )
            throw new JsonException($" Missing required  typeName  property for ConstantNode: \n{root.GetRawText()}");

        return new ConstantNode() { ValueType = valueType, Value = value };
    }

    public override void Write(Utf8JsonWriter writer, ConstantNode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$type", "val");
        writer.WriteString("type", value.ValueType?.Name ?? "null");
        writer.WritePropertyName("value");
        JsonSerializer.Serialize(writer, value.Value, options);
        writer.WriteEndObject();
    }

    private object? ConvertJsonElement(JsonElement element, Type targetType, JsonSerializerOptions serializerOptions)
    {
        var valueOptions = ValueConvertOptions;
        return JsonValueTypeParser.ConvertJsonElement(element, targetType, valueOptions);
    }

}

#endregion
