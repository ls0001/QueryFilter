using System.Text.Json;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor.Json;

#region JSON Converter
/// <summary>
/// Json节点转换器
/// </summary>
public class QueryNodeJsonConverter : JsonConverter<QueryNode>
{

    public override QueryNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        try
        {
            var targetType = NodeJsonConverterTypeRegistry.GetConcreteType(root, options);
            return (QueryNode)JsonSerializer.Deserialize(root, targetType, options);
        }
        catch ( Exception ex )
        {
            throw new JsonException($"{ex.Message}\n{root.GetRawText()}", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, QueryNode value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }

}
#endregion


