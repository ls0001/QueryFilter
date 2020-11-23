using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal.JsonConvert
{
    public class QueryItemJsonConverter : JsonConverter<QueryItem>
    {

        public override QueryItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeToConvert);
            return new ValueItem();

        }
        public override void Write(Utf8JsonWriter writer, QueryItem value, JsonSerializerOptions options)
        {
           // var converter = options.GetConverter(value.GetType());
            JsonSerializer.Serialize(value, value.GetType(), options);
        }
    }
}
