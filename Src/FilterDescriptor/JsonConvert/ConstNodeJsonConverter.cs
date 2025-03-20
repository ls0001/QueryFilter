using System.Text.Json.Serialization;
using System.Text.Json;



namespace DynamicQuery.Descriptor;


#region Constant Node Converter

/// <summary>
/// 常量节点转换器
/// </summary>
public class ConstNodeJsonConverter : JsonConverter<ConstantNode>
{
    public  JsonValueConvertOptions valueOptions { get; set; } = new();
    public override ConstantNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var node = new ConstantNode();

        if ( root.TryGetProperty("type", out var typeProp) )
            node.TypeName = typeProp.GetString();

        if ( root.TryGetProperty("value", out var valueProp) )
            node.Value = ConvertJsonElement(valueProp, node.TypeName, options);
        else if ( root.TryGetProperty("val", out var valProp) )
            node.Value = ConvertJsonElement(valProp, node.TypeName, options);

        return node;
    }

    private object ConvertJsonElement(JsonElement element, string typeName, JsonSerializerOptions serializerOptions)
    {
        var valueConvertOptions = valueOptions; //serializerOptions.GetQueryOptions();
        var targetType = valueOptions.GetTypeWinthName(typeName);
        return JsonValueTypeConverter.ConvertJsonElement(element, targetType, valueConvertOptions);
    }

    public override void Write(Utf8JsonWriter writer, ConstantNode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$type", "val");
        writer.WriteString("type", value.TypeName);
        writer.WritePropertyName("value");
        JsonSerializer.Serialize(writer, value.Value, options);
        writer.WriteEndObject();
    }
}

#endregion
