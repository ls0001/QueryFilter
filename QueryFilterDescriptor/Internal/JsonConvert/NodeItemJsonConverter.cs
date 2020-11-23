using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    internal sealed class NodeItemJsonConverter : JsonConverter<ValueItem>
    {
        private readonly JsonEncodedText _nodeItem_TypeAsPtyName = JsonEncodedText.Encode(Constants.Const_NodeItem_TypeAsPtyName);   // nameof(NodeItem.TypeAs);
        //private readonly JsonEncodedText _nodeItem_FlagPtyName = JsonEncodedText.Encode(Global.Const_QueryItem_FlagPtyName);       // nameof(NodeItem.Flag);
        private readonly JsonEncodedText _nodeItem_ValuePtyName = JsonEncodedText.Encode(Constants.Const_NodeItem_ValuePtyName);     // nameof(NodeItem.Value);
        private readonly JsonEncodedText _valueTypeName = JsonEncodedText.Encode("ValueType");

        private readonly Type _typeOfObject = Constants.Static_TypeOfObject;                        //typeof(object);
        private readonly Type _typeOfString = Constants.Static_TypeOfString;
        private readonly Dictionary<string, Type> _valueTypeCacheDict = new Dictionary<string, Type>();
        //   private readonly Dictionary<string, Type> _valueTypeAsCacheDict = new Dictionary<string, Type>();
        public override ValueItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)              //{
                throw new JsonException(MsgStrings.NotJsonObjectStart);

            var typeAs = _typeOfObject;
            var itemValue = default(JsonElement);
            var item = new ValueItem();
            Type valueType = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)    //进入属性
            {
                if (!(reader.TokenType == JsonTokenType.PropertyName))
                    continue;
                var jsonPtyName = reader.ValueSpan;
                if (!reader.Read())    //进入属性值
                    continue;

                if (jsonPtyName.SequenceEqual(_nodeItem_TypeAsPtyName.EncodedUtf8Bytes))
                {
                    var typeName = reader.GetString();
                    if (options.IgnoreNullValues || !string.IsNullOrWhiteSpace(typeName))
                    {
                        if (!_valueTypeCacheDict.TryGetValue(typeName, out typeAs))
                        {
                            typeAs = Type.GetType(typeName);
                            _valueTypeCacheDict.Add(typeName, typeAs);
                        }
                        item.TypeAs = typeAs;
                    }
                }
                else
                if (jsonPtyName.SequenceEqual(_valueTypeName.EncodedUtf8Bytes))
                {
                    var typeName = reader.GetString();
                    if (!string.IsNullOrWhiteSpace(typeName))
                    {
                        if (!_valueTypeCacheDict.TryGetValue(typeName, out valueType))
                        {
                            valueType = Type.GetType(typeName);
                            _valueTypeCacheDict.Add(typeName, valueType);
                        }
                    }
                }
                else
                if (jsonPtyName.SequenceEqual(_nodeItem_ValuePtyName.EncodedUtf8Bytes))
                {
                    using var valueDoc = JsonDocument.ParseValue(ref reader);
                    itemValue = valueDoc.RootElement.Clone();
                }
                //else
                //if (jsonPtyName.SequenceEqual(_nodeItem_FlagPtyName.EncodedUtf8Bytes))  //兼顾第三方Json
                //{
                //    hasFlag = true;
                //    if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var fg))
                //        item.Flag = (NodeItemFlag)fg;
                //    else if (reader.TokenType == JsonTokenType.String
                //         && (Enum.TryParse(reader.GetString(), ignoreCase: false, out NodeItemFlag fgstr)
                //         || Enum.TryParse(reader.GetString(), ignoreCase: true, out fgstr)))
                //        item.Flag = fgstr;
                //    else
                //        hasFlag = false;
                //}
            }
            if (itemValue.ValueKind != JsonValueKind.Undefined)
            {
                if (valueType != null) //当使用本转换器：如果valueType!=null，必定ItemValue!=null,
                    item.Value = JsonSerializer.Deserialize(itemValue.GetRawText(), valueType, options);
                else if (itemValue.ValueKind != JsonValueKind.Null)
                {//处理三方标准JSON，无附加的类型说明，
                    if (item.Flag == NodeItemFlag.Property)
                        valueType = _typeOfString;  //属性名称一定是字符串类型
                    else  //直接反序列化到目标强制类型,或者默认为object类型
                    {
                        valueType = typeAs ?? _typeOfObject;  //如果typeAs==null,默认为object类型
                    }

                    item.Value = JsonSerializer.Deserialize(itemValue.GetRawText(), valueType, options);

                }
            }
            return item;

        }

        public override void Write(Utf8JsonWriter writer, ValueItem item, JsonSerializerOptions options)
        {
            var typeAs = item?.TypeAs?.FullName;
            //var flag = (int)item?.Flag;

            var val = item?.Value;
            writer.WriteStartObject();
            {
                if (typeAs != null)
                    writer.WriteString(_nodeItem_TypeAsPtyName, typeAs);
                //if (!options.IgnoreNullValues || val != null)
                if (val != null)
                {
                    var valueType = val?.GetType();
                    writer.WriteString(_valueTypeName, valueType?.FullName);   //因为NodeItem.Value定义为object类型，附加类型说明以便反序列化
                    //writer.WritePropertyName(_nodeItem_ValuePtyName); JsonSerializer.Serialize(writer, val, valueType, options);
                }
                //if (!options.IgnoreNullValues || typeAs != null)
                //writer.WriteNumber(_nodeItem_FlagPtyName, flag);
            }
            writer.WriteEndObject();
        }
    }
}
