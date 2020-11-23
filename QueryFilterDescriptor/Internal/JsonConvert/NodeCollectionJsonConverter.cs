using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    internal sealed class NodeCollectionJsonConverter : JsonConverterFactory
    {
        private readonly Type _typeOfNodeCollectionOpenGeneric = Constants.Static_TypeOfNodeCollectionOpenGeneric;
        private readonly Type _typeOfQueryNode = Constants.Static_TypeOfQueryNode;
        private readonly Type _typeofNodeCollectionConverterInnerOpenGeneric = typeof(NodeCollectionConverterInner<,>);
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.GetGenericTypeDefinition() != _typeOfNodeCollectionOpenGeneric)
                return false;

            var args = typeToConvert.GetGenericArguments();
            return args.Length==2 && args[0].IsEnum && _typeOfQueryNode.IsAssignableFrom(args[1]);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

            var converter = (JsonConverter)Activator.CreateInstance(
                _typeofNodeCollectionConverterInnerOpenGeneric.MakeGenericType(new Type[] { keyType, valueType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        private class NodeCollectionConverterInner<TOperator, TNode> : JsonConverter<NodeCollection<TOperator, TNode>>
            where TOperator : struct, Enum
            where TNode : QueryNode //,IEquatable<TNode>
        {
            private readonly string _nodePair_OperatorPtyName = Constants.Const_NodeOperatorPair_OperatorPtyName;    // nameof(NodeOperatorPair<TOperator, TNode>.Operator);
            private readonly string _nodePair_ValuePtyName = Constants.Const_NodeOperatorPair_NodePtyName;         // nameof(NodeOperatorPair<TOperator, TNode>.Node);

            private readonly Type _OperatorType;
            private readonly Type _NodeType;

            private readonly JsonConverter<TNode> _valueConverter;

            public NodeCollectionConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _valueConverter = (JsonConverter<TNode>)options
                    .GetConverter(typeof(TNode));

                // Cache the key and value types.
                _OperatorType = typeof(TOperator);
                _NodeType = typeof(TNode);
            }

            public override NodeCollection<TOperator, TNode> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException(MsgStrings.NotJsonArrayStart);
                }


                NodeCollection<TOperator, TNode> nodes = new NodeCollection<TOperator, TNode>();
                var nodelist = new InternalList<NodeOperatorPair<TOperator, TNode>>();
                while (reader.Read()) //进入{ 或 ]
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        nodes.Items = nodelist.ToArray(0);//insert into NodeCollecton
                        return nodes;
                    }

                    // Get the operator.
                    if (reader.TokenType != JsonTokenType.StartObject)
                        //throw new JsonException(MsgStrings.NotJsonObjectStart);
                        continue;

                    if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)  //进入属性
                        continue; //必须是属性，因为这是键值对NodeOperatorPair<,>对象

                    ////if (reader.TokenType != JsonTokenType.PropertyName) //必须是属性，因为这是键值对NodeOperatorPair<,>对象
                    ////    throw new JsonException(string.Format(MsgStrings.JsonTokenTypeNotPropertyName, reader.TokenType.ToString()));

                    TOperator op;
                    string propertyName = reader.GetString();
                    if (int.TryParse(propertyName, out int iop))//非标准Json,本转换器的优化算法json
                        op = (TOperator)(object)iop;
                    else if (// For performance, parse with ignoreCase:false first.
                        (Enum.TryParse(propertyName, ignoreCase: false, out op) || Enum.TryParse(propertyName, ignoreCase: true, out op)))
                    { }//no code;
                    else if (propertyName == _nodePair_OperatorPtyName && reader.Read())  //如果是标准JSON,属性名称为键值对的key本名（Operator），并且进入读取Operator属性值
                    {
                        //读取到的属性值能够转换为TOperator
                        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int iop_standar))
                        {
                            op = (TOperator)(object)iop_standar;
                        }
                        else if (reader.TokenType == JsonTokenType.String
                            && (Enum.TryParse(reader.GetString(), ignoreCase: false, out op) || Enum.TryParse(reader.GetString(), ignoreCase: true, out op)))
                        { } //no code;
                        else
                        {
                            //非TOperator类型的值
                            throw new JsonException();
                        }
                        if (!(reader.Read() && reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == _nodePair_ValuePtyName))
                        {//标准Json key("Operator")属性值 之后 必须是value（"Node")属性,否则错误
                            throw new JsonException();
                        }

                    }
                    else
                    {
                        throw new JsonException(MsgStrings.JsonCannotConvertToType( propertyName, _OperatorType));
                        //$"Unable to convert \"{propertyName}\" to Operator \"{_OperatorType}\"."); 
                    }

                    // Get the node.
                    TNode node;
                    if (_valueConverter != null)
                    {
                        reader.Read(); //进入{
                        node = _valueConverter.Read(ref reader, _NodeType, options);
                    }
                    else
                    {
                        node = JsonSerializer.Deserialize<TNode>(ref reader, options);
                    }

                    // Add to NoCollection innerList.
                    nodelist.Add(new NodeOperatorPair<TOperator, TNode>(op, node));
                    reader.Read(); //进入}
                }

                throw new JsonException(MsgStrings.JsonDeserializeCollectionHasError);
            }

            public override void Write(
                Utf8JsonWriter writer,
                NodeCollection<TOperator, TNode> Nodes,
                JsonSerializerOptions options)
            {

                writer.WriteStartArray();
                foreach (NodeOperatorPair<TOperator, TNode> item in Nodes)
                {
                    writer.WriteStartObject();
                    //// writer.WritePropertyName(item.Operator.ToString());
                    writer.WritePropertyName(Convert.ToInt32(item.Operator).ToString());
                    if (_valueConverter != null)
                    {
                        _valueConverter.Write(writer, item.Node, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, item.Node, options);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

            }
        }
    }

}

