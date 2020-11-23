using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Sag.Data.Common.Query.Internal
{
    public sealed class ItemOperatorJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (typeToConvert.GetGenericTypeDefinition() != typeof(NodeOperatorPair<,>))
                return false;

            var args = typeToConvert.GetGenericArguments();
            return args?.Length == 2
                && args[0].IsEnum
                && typeof(QueryItem).IsAssignableFrom(args[1]);

        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            var args = type.GetGenericArguments();
            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(NodeOperatorPairConverterInner<,>).MakeGenericType(args),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        private class NodeOperatorPairConverterInner<TOperator, TNode> :
            JsonConverter<NodeOperatorPair<TOperator, TNode>>
            where TNode : QueryNode 
            where TOperator : struct, Enum
        {
            private readonly JsonConverter<TNode> _nodeConverter;
            //private Type _OperatorType;
            private Type _nodeType;

            public NodeOperatorPairConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _nodeConverter = (JsonConverter<TNode>)options.GetConverter(typeof(TNode));
                //_nodeConverter = (JsonConverter<TNode>)new NodeBlockJsonConverter().CreateConverter(typeof(TNode), options);
                //_itemConverter=JsonConverter()
                // Cache the key and value types.
                _nodeType = typeof(TNode);
              //  _OperatorType = typeof(TOperator);

            }

            public override NodeOperatorPair<TOperator, TNode> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("不是Json对象");
                }
                NodeOperatorPair<TOperator, TNode> pair = new NodeOperatorPair<TOperator, TNode>();
                var nodeName = nameof(pair.Node);
                var opName = nameof(pair.Operator);
                while (reader.Read() && reader.TokenType!= JsonTokenType.EndObject)
                {
                    if (!(reader.TokenType == JsonTokenType.PropertyName))
                        continue;
                    var ptyName = reader.GetString();
                        if (!reader.Read())
                            continue;

                    if (ptyName == opName)
                    {
                        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var op))
                            pair.Operator = (TOperator)(object)op;
                        else if (reader.TokenType == JsonTokenType.String &&
                            (Enum.TryParse(reader.GetString(), ignoreCase: false, out TOperator opstr) ||
                             Enum.TryParse(reader.GetString(), ignoreCase: true, out opstr)))
                            pair.Operator = opstr;
                    }
                    if (ptyName == nodeName)
                    {
                        if (_nodeConverter != null)
                            pair.Node = _nodeConverter.Read(ref reader, _nodeType, options);
                        else
                            pair.Node = JsonSerializer.Deserialize<TNode>(ref reader, options);
                    }
                }

                return pair;           
            }

            public override void Write(
                Utf8JsonWriter writer,
                NodeOperatorPair<TOperator, TNode> pair,
                JsonSerializerOptions options)
            {

                writer.WriteStartObject();
                writer.WriteNumber(nameof(pair.Operator), (int)(object)pair.Operator);
                writer.WritePropertyName(nameof(pair.Node));
                if (_nodeConverter != null)
                {
                    _nodeConverter.Write(writer, pair.Node, options);
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

