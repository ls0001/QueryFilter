using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    internal sealed class NodeBlockJsonConverter : JsonConverterFactory
    {
        private readonly Type _typeOfQueryItem = Constants.Static_TypeOfQueryItem;
        private readonly Type _typeofQueryBlockOpenGeneric = Constants.Static_TypeofQueryBlockOpenGeneric;
        private readonly Type _typeOfNodeBlockConvertrInnerOpenGeneric = typeof(NodeBlockConverterInner<,,>);
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.BaseType?.GetGenericTypeDefinition() != _typeofQueryBlockOpenGeneric)
                return false;

            var args = typeToConvert.BaseType?.GetGenericArguments();
            return args?.Length == 3
                && _typeOfQueryItem.IsAssignableFrom(args[0])
                && args[1].BaseType?.GetGenericTypeDefinition() == _typeofQueryBlockOpenGeneric
                && args[2].IsEnum;
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            var args = type.BaseType.GetGenericArguments();

            var converter = (JsonConverter)Activator.CreateInstance(
                _typeOfNodeBlockConvertrInnerOpenGeneric.MakeGenericType(args),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        internal class NodeBlockConverterInner<TItem, TBlock, TOperator> :
            JsonConverter<QueryBlock<TItem, TBlock, TOperator>>
            where TItem : QueryItem //,IEquatable<TItem> 
            where TBlock : QueryBlock<TItem, TBlock, TOperator>
            where TOperator : struct, Enum
        {

            // Cache the item and block types.
            private readonly Type _blockType = typeof(TBlock);
            //private readonly Type _blockOperatorArrayPairType = typeof(NodeOperatorPair<TOperator, TBlock>[]);
            private readonly Type _itemType = typeof(TItem);
            //private readonly Type _itemOperatorPairType = typeof(NodeOperatorPair<TOperator,TItem>);
            private readonly Type _itemOperatorPairArrayType = typeof(NodeOperatorPair<TOperator, TItem>[]);

            private readonly JsonEncodedText _block_AutoReducePtyName =JsonEncodedText.Encode(Constants.Const_QueryBlock_AutoReducePtyName); // nameof(QueryBlock<TItem, TBlock, TOperator>.AutoReduce);
            private readonly JsonEncodedText _block_TypeAsPtynName = JsonEncodedText.Encode(Constants.Const_QueryBlock_TypeAsPtyName); // nameof(QueryBlock<TItem, TBlock, TOperator>.TypeAs);
            private readonly JsonEncodedText _block_ItemsPtyName = JsonEncodedText.Encode(Constants.Const_QueryBlock_ItemsPtyName); //nameof(QueryBlock<TItem, TBlock, TOperator>.Items);
            private readonly JsonEncodedText _block_blocksPtyName = JsonEncodedText.Encode(Constants.Const_QueryBlock_BlocksPtyName); // nameof(QueryBlock<TItem, TBlock, TOperator>.Blocks);
            private readonly JsonEncodedText _NodePair_OperatorPtyName = JsonEncodedText.Encode(Constants.Const_NodeOperatorPair_OperatorPtyName);     // nameof(NodeOperatorPair<TOperator, TItem>.Operator);
            private readonly JsonEncodedText _NodePair_NodePtyName = JsonEncodedText.Encode(Constants.Const_NodeOperatorPair_NodePtyName);   // nameof(NodeOperatorPair<TOperator, TItem>.Node);

            private JsonConverter<NodeOperatorPair<TOperator, TItem>[]> _itemArrayConverter;
            private JsonConverter<TItem> _itemConverter;
            private NodeBlockConverterInner<TItem, TBlock, TOperator> _blockConverter;

            private readonly Dictionary<string, Type> _valueTypeAsCacheDict = new Dictionary<string, Type>();
            public NodeBlockConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _itemArrayConverter = (JsonConverter<NodeOperatorPair<TOperator, TItem>[]>)options.GetConverter(_itemOperatorPairArrayType);
                _itemConverter = (JsonConverter<TItem>)options.GetConverter(typeof(TItem));
            }

            public override QueryBlock<TItem, TBlock, TOperator> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject) //{
                {
                    throw new JsonException(MsgStrings.NotJsonObjectStart);
                }
                _blockConverter = (NodeBlockConverterInner<TItem, TBlock, TOperator>)options.GetConverter(_blockType);
                if (_blockConverter == null)
                    _blockConverter = new NodeBlockConverterInner<TItem, TBlock, TOperator>(options);

                var items = default(NodeOperatorPair<TOperator, TItem>[]);
                var subblocks = new InternalList<NodeOperatorPair<TOperator, TBlock>>();
                var autoReduce = true;
                Type typeAs = null;
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)    //进入属性名
                {
                    if (!(reader.TokenType == JsonTokenType.PropertyName))
                        continue;
                    var propName = reader.ValueSpan;

                    if (!reader.Read())                                                 //进入属性值
                        continue;

                    //读取属性值
                    if (propName.SequenceEqual( _block_AutoReducePtyName.EncodedUtf8Bytes))
                    {
                        autoReduce = reader.GetBoolean();
                    }else
                    if (propName.SequenceEqual(_block_TypeAsPtynName.EncodedUtf8Bytes))
                    {
                        var tpAsName = reader.GetString();
                        if (!string.IsNullOrWhiteSpace(tpAsName))
                        {
                            if (!_valueTypeAsCacheDict.TryGetValue(tpAsName, out typeAs))
                            {
                                typeAs = Type.GetType(tpAsName);
                                _valueTypeAsCacheDict.Add(tpAsName, typeAs);
                            }
                        }
                    }else
                    if (propName.SequenceEqual(_block_ItemsPtyName.EncodedUtf8Bytes))
                    {
                        if (_itemArrayConverter != null)
                            items = _itemArrayConverter.Read(ref reader, _itemOperatorPairArrayType, options);  //items数组
                        else
                            items = (NodeOperatorPair<TOperator, TItem>[])JsonSerializer.Deserialize(ref reader, _itemOperatorPairArrayType, options);

                    }else
                    if (propName.SequenceEqual(_block_blocksPtyName.EncodedUtf8Bytes))
                    {
                        if (reader.TokenType != JsonTokenType.StartArray)                   //[
                            throw new JsonException(MsgStrings.NotJsonArrayStart);

                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)//{ 每个块
                        {
                            if (reader.TokenType == JsonTokenType.StartObject)
                            {
                                var hasOp = false;
                                // var hasNode = false;
                                var pair = new NodeOperatorPair<TOperator, TBlock>();
                                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)//每个属性
                                {
                                    if (!(reader.TokenType == JsonTokenType.PropertyName))
                                        continue;

                                    var ptyName = reader.ValueSpan;
                                    if (!reader.Read()) //进入属性值
                                        continue;
                                   if(ptyName.SequenceEqual(_NodePair_OperatorPtyName.EncodedUtf8Bytes))
                                    {
                                        hasOp = true;
                                        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var op))
                                            pair.Operator = (TOperator)(object)op;
                                        else if (reader.TokenType == JsonTokenType.String &&
                                            (Enum.TryParse(reader.GetString(), ignoreCase: false, out TOperator opstr) ||
                                             Enum.TryParse(reader.GetString(), ignoreCase: true, out opstr)))
                                            pair.Operator = opstr;
                                        else
                                            hasOp = false;
                                    }else
                                    if (ptyName.SequenceEqual(_NodePair_NodePtyName.EncodedUtf8Bytes))
                                    {
                                        if (_blockConverter != null)
                                            pair.Node = (TBlock)_blockConverter.Read(ref reader, _blockType, options);
                                        else
                                            pair.Node = (TBlock)JsonSerializer.Deserialize(ref reader, _blockType, options);
                                    }
                                }
                                if (hasOp && pair.Node?.AllCount > 0)
                                    subblocks.Add(pair);
                            }
                        }
                    }
                }
                var iCount = items?.Length;
                var bCount = subblocks.Count;
                if (iCount + bCount > 0)
                {
                    var blocks = (QueryBlock<TItem, TBlock, TOperator>)Activator.CreateInstance(typeToConvert);
                    blocks.TypeAs = typeAs;
                    blocks.AutoReduce = autoReduce;
                    if (iCount > 0)
                        blocks.Items = items;//由block自动排除null的item
                    if (bCount > 0)
                        blocks.Blocks = subblocks.ToArray();
                    if (blocks.AllCount > 0)
                        return blocks;
                }
                return null;
            }


            public override void Write(
                Utf8JsonWriter writer,
                QueryBlock<TItem, TBlock, TOperator> blocks,
                JsonSerializerOptions options)
            {
                _blockConverter = (NodeBlockConverterInner<TItem, TBlock, TOperator>)options.GetConverter(_blockType);
                if (_blockConverter == null)
                    _blockConverter = new NodeBlockConverterInner<TItem, TBlock, TOperator>(options);


                writer.WriteStartObject();
                {
                    writer.WriteBoolean(_block_AutoReducePtyName, blocks.AutoReduce);//nameof(blocks.AutoReduce)
                    writer.WritePropertyName(_block_ItemsPtyName);//nameof(blocks.Items)
                    {
                        writer.WriteStartArray();
                        foreach (var item in blocks.GetItems())
                        {
                            writer.WriteStartObject();  //: NodeOperatorPair<TOperater,TItem>
                            {
                                writer.WriteNumber(_NodePair_OperatorPtyName, (int)(object)item.Operator);     //: "Operator":1
                                writer.WritePropertyName(_NodePair_NodePtyName);
                                if (_itemConverter != null)
                                {
                                    _itemConverter.Write(writer, item.Node, options);
                                }
                                else
                                {
                                    JsonSerializer.Serialize(writer, item.Node, options);
                                }
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                    }
                    writer.WritePropertyName(_block_blocksPtyName);
                    {
                        writer.WriteStartArray();
                        foreach (var item in blocks.GetBlocks())
                        {
                            writer.WriteStartObject();
                            {
                                writer.WriteNumber(_NodePair_OperatorPtyName, (int)(object)item.Operator);
                                writer.WritePropertyName(_NodePair_NodePtyName);
                                if (_blockConverter != null)
                                {
                                    _blockConverter.Write(writer, item.Node, options);
                                }
                                else
                                {
                                    JsonSerializer.Serialize(writer, item.Node, options);
                                }
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                    }
                    writer.WriteString(_block_TypeAsPtynName, blocks.TypeAs?.FullName);
                }
                writer.WriteEndObject();

            }
        }
    }

}

