using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    internal sealed class PropertyItemJsonConverter : JsonConverter<PropertyItem>
    {
        private readonly JsonEncodedText _nodeItem_NamePtyName = JsonEncodedText.Encode(Constants.Const_PropertyItem_NamePtyName);     // nameof(NodeItem.Value);
        private readonly JsonEncodedText _nodeItem_TypeAsPtyName = JsonEncodedText.Encode(Constants.Const_PropertyItem_TypeAsPtyName);   // nameof(NodeItem.TypeAs);
        private readonly JsonEncodedText _nodeItem_ClassTypeName = JsonEncodedText.Encode(Constants.Const_PropertyItem_ClassTypeName);

        private readonly Dictionary<string, Type> _valueTypeCacheDict = new Dictionary<string, Type>();
        //   private readonly Dictionary<string, Type> _valueTypeAsCacheDict = new Dictionary<string, Type>();
        public override PropertyItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)              //{
                throw new JsonException(MsgStrings.NotJsonObjectStart);

            var item = new PropertyItem();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)    //进入属性
            {
                if (!(reader.TokenType == JsonTokenType.PropertyName))
                    continue;
                var jsonPtyName = reader.ValueSpan;
                if (!reader.Read())    //进入属性值
                    continue;

                if (jsonPtyName.SequenceEqual(_nodeItem_ClassTypeName.EncodedUtf8Bytes))
                {
                    var typeName = reader.GetString();
                    if (!string.IsNullOrWhiteSpace(typeName))
                    {
                        if (!_valueTypeCacheDict.TryGetValue(typeName, out Type clsType))
                        {
                            clsType = Type.GetType(typeName);
                            _valueTypeCacheDict.Add(typeName, clsType);
                        }
                        item.ClassType = clsType;
                    }
                }
                else
                if (jsonPtyName.SequenceEqual(_nodeItem_TypeAsPtyName.EncodedUtf8Bytes))
                {
                    var typeName = reader.GetString();
                    if (options.IgnoreNullValues || !string.IsNullOrWhiteSpace(typeName))
                    {
                        if (!_valueTypeCacheDict.TryGetValue(typeName, out Type typeAs))
                        {
                            typeAs = Type.GetType(typeName);
                            _valueTypeCacheDict.Add(typeName, typeAs);
                        }
                        item.TypeAs = typeAs;
                    }
                }
                else
                if (jsonPtyName.SequenceEqual(_nodeItem_NamePtyName.EncodedUtf8Bytes))
                {
                    item.Name = reader.GetString();
                }

            }
            return item;
        }

        public override void Write(Utf8JsonWriter writer, PropertyItem item, JsonSerializerOptions options)
        {
            var typeAs = item?.TypeAs?.FullName;
            var classType = item?.ClassType?.FullName;
            var ptyName = item?.Name;
            if (string.IsNullOrWhiteSpace(ptyName) || classType==null)
                return;

            writer.WriteStartObject();
            {
                 //if (!options.IgnoreNullValues || classType != null)
                if (classType != null)
                    writer.WriteString(_nodeItem_ClassTypeName, classType);
                //if (!options.IgnoreNullValues || typeAs != null)
                if (typeAs != null)
                    writer.WriteString(_nodeItem_TypeAsPtyName, typeAs);                
                //if (!options.IgnoreNullValues || ptyName != null)
                if (ptyName != null)
                    writer.WriteString(_nodeItem_NamePtyName, ptyName);


            }
            writer.WriteEndObject();
        }
    }
}
