using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    internal sealed class TypeToJsonConverter : JsonConverter<Type>
    {
        private readonly Dictionary<string, Type> _typeCacheDict = new Dictionary<string, Type>();
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                var tpName = reader.GetString();
                if (!string.IsNullOrWhiteSpace(tpName) )
                {
                    if (!_typeCacheDict.TryGetValue(tpName, out var tp))
                    {
                        tp = Type.GetType(tpName);
                        _typeCacheDict.Add(tpName, tp);
                    }
                    return tp;
                }
                //throw new InvalidCastException(string.Format( MsgStrings.InvalidTypeConvert,tpName));
            }
            return typeof(object);
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }

}

