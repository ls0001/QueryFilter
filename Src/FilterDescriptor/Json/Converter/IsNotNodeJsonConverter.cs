using System.Text.Json.Serialization;
using System.Text.Json;


namespace DynamicQuery.Descriptor.Json;

#region Constant Node Converter

/// <summary>
/// 常量节点转换器
/// </summary>
public class IsNotNodeJsonConverter : JsonConverter<NotNode>
{
    public override NotNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        QueryNode bodyNode;
        if ( root.TryGetProperty("body", out var bodyProp) )
            bodyNode = JsonSerializer.Deserialize<QueryNode>(bodyProp.GetRawText(), options);
        else
        {
            var bodyJson = root.GetProperty("not");
            bodyNode = JsonSerializer.Deserialize<QueryNode>(bodyJson, options);
        }
        if ( bodyNode != null )
        {
            return new NotNode { Body = bodyNode };
        }
        else
        {
            throw new JsonException($" Missing required  body property for IsNotNode: \n{root.GetRawText()}");
        }
    }

    public override void Write(Utf8JsonWriter writer, NotNode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$type", "not");
        writer.WritePropertyName("body");
        JsonSerializer.Serialize(writer, value.Body, options);
        writer.WriteEndObject();
    }
}

#endregion
