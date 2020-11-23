using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    public sealed class NodeOperatorJsonConverter : JsonConverterFactory
    {
        private readonly Type _typeOfNodeOperatorPairOpenGeneric = Constants.Static_TypeOfNodeOperatorOpenGeneric;  //typeof(NodeOperatorPair<,>)
        private readonly Type _typeOfQueryNode = Constants.Static_TypeOfQueryNode;

        private readonly Type _typeOfNodeOperatorPairConverterInnerOpenGeneric = typeof(NodeOperatorPairConverterInner<,>);
       
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.GetGenericTypeDefinition() != _typeOfNodeOperatorPairOpenGeneric)
                return false;

            var args = typeToConvert.GetGenericArguments();
            return args?.Length == 2
                && args[0].IsEnum
                && _typeOfQueryNode.IsAssignableFrom(args[1]);

        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {

            var args = type.GetGenericArguments();
            var converter = (JsonConverter)Activator.CreateInstance(
                _typeOfNodeOperatorPairConverterInnerOpenGeneric.MakeGenericType(args),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        private class NodeOperatorPairConverterInner<TOperator, TNode> :
            JsonConverter<NodeOperatorPair<TOperator, TNode>>
            where TNode : QueryNode //,IEquatable<TNode>
            where TOperator : struct, Enum
        {
            private readonly string _nodePair_NodePtyName = Constants.Const_NodeOperatorPair_NodePtyName;           //  nameof(pair.Node);
            private readonly string _nodePair_OperatorPtyName = Constants.Const_NodeOperatorPair_OperatorPtyName;   // nameof(pair.Operator);           

            private readonly JsonConverter _nodeConverter;
            private readonly Type _nodeType = typeof(TNode);

            nodeReadDelegate _readMethodDelegate;
            nodeWriteDelegate _writeMethodDelegate;

            delegate QueryNode nodeReadDelegate(ref Utf8JsonReader reader, Type toConvert, JsonSerializerOptions options);
            delegate void nodeWriteDelegate(Utf8JsonWriter writer, TNode pair, JsonSerializerOptions options);

            public NodeOperatorPairConverterInner(JsonSerializerOptions options)
            {

                // For performance, use the existing converter if available.
                _nodeConverter = options.GetConverter(_nodeType);
                if (_nodeConverter == null)
                    return;
                var readMethod = _nodeConverter.GetType().GetMethod("Read");
                var writeMethod = _nodeConverter.GetType().GetMethod("Write");
                _readMethodDelegate = (nodeReadDelegate)readMethod.CreateDelegate(typeof(nodeReadDelegate), _nodeConverter);
                _writeMethodDelegate = (nodeWriteDelegate)writeMethod.CreateDelegate(typeof(nodeWriteDelegate), _nodeConverter);

            }


            public override NodeOperatorPair<TOperator, TNode> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException(MsgStrings.NotJsonObjectStart);
                }
                NodeOperatorPair<TOperator, TNode> pair = new NodeOperatorPair<TOperator, TNode>();

                var hasOp = false;
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (!(reader.TokenType == JsonTokenType.PropertyName))
                        continue;
                    var ptyName = reader.GetString();
                    if (!reader.Read())
                        continue;

                    if (ptyName == _nodePair_OperatorPtyName)
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
                    if (ptyName == _nodePair_NodePtyName)
                    {
                        if (_nodeConverter != null)
                            pair.Node = (TNode)_readMethodDelegate(ref reader, _nodeType, options);
                        else
                            pair.Node = JsonSerializer.Deserialize<TNode>(ref reader, options);
                    }
                }
                if (hasOp && pair.Node != null)
                    return pair;
                else
                    return null;
            }

            public override void Write(
                Utf8JsonWriter writer,
                NodeOperatorPair<TOperator, TNode> pair,
                JsonSerializerOptions options)
            {
                if (pair.Node == null)
                    return;
                writer.WriteStartObject();
                writer.WriteNumber(_nodePair_OperatorPtyName, (int)(object)pair.Operator);   // nameof(pair.Operator)
                writer.WritePropertyName(_nodePair_NodePtyName);                             // nameof(pair.Node)
                if (_nodeConverter != null)
                {
                    _writeMethodDelegate(writer, pair.Node, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, pair.Node, options);
                }
                writer.WriteEndObject();


            }
        }
    }

}

