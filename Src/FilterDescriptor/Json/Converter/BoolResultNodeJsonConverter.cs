using System.Text.Json;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor.Json;

public class BoolResultNodeJsonConverter : JsonConverter<IBoolResultNode>
{

    public override IBoolResultNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader);

        var root = doc.RootElement;
        return (IBoolResultNode)JsonSerializer.Deserialize(root, NodeJsonConverterTypeRegistry.GetConcreteType(root, options), options)!;
    }

    public override void Write(Utf8JsonWriter writer, IBoolResultNode value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }

}
