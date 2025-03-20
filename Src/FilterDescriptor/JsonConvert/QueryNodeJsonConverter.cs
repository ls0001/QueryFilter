using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor
{
 

    #region JSON Converter
    /// <summary>
    /// Json节点转换器
    /// </summary>
    public class QueryNodeJsonConverter : JsonConverter<QueryNode>
    {
        private static readonly ImmutableDictionary<string, Type> _nodeTypeMap =
        ImmutableDictionary<string, Type>.Empty
        .WithComparers(StringComparer.OrdinalIgnoreCase)
        .AddRange(
        [
            new (nameof(QueryNodeType.cond), typeof(ConditionNode)),
            new(nameof(QueryNodeType.eq), typeof(EqualNode)),
            new(nameof(QueryNodeType.lt), typeof(LessThanNode)),
            new(nameof(QueryNodeType.gt), typeof(GreaterThanNode)),
            new(nameof(QueryNodeType.lte), typeof(LessThanOrEqualNode)),
            new(nameof(QueryNodeType.gte), typeof(GreaterThanOrEqualNode)),
            new(nameof(QueryNodeType.add), typeof(AddNode)),
            new(nameof(QueryNodeType.sub), typeof(SubtractNode)),
            new(nameof(QueryNodeType.mul), typeof(MultiplyNode)),
            new(nameof(QueryNodeType.div), typeof(DivideNode)),
            new(nameof(QueryNodeType.mod), typeof(ModuloNode)),
            new(nameof(QueryNodeType.like), typeof(LikeNode)),
            new(nameof(QueryNodeType.btw), typeof(BetweenNode)),       
            new(nameof(QueryNodeType.@in), typeof(InNode)),          
            new(nameof(QueryNodeType.field), typeof(FieldNode)),
            new(nameof(QueryNodeType.val), typeof(ConstantNode)),    
            new(nameof(QueryNodeType.not), typeof(NotNode)),
            new(nameof(QueryNodeType.nan), typeof(IsNullNode)),
            new(nameof(QueryNodeType.func), typeof(FuncNode)),
            new(nameof(QueryNodeType.concat), typeof(ConcatNode)),
            new(nameof(QueryNodeType.pf), typeof(StartWithNode)),
            new(nameof(QueryNodeType.sf), typeof(EndWithNode)),
            new(nameof(QueryNodeType.sfunc), typeof(SimpleFuncNode)),
        ]);

        public override QueryNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            string typeName = null;
            if ( root.TryGetProperty("$type", out var typeProp) )
            {
                typeName = typeProp.GetString();
            }
            else
            if ( root.TryGetProperty("not", out var notProp) && (notProp.TryGetProperty("$type" , out _)))    
                    typeName = "not";
            else
            {
                throw new JsonException($"Missing required $type property  {root.GetRawText()}");
            }
            
            if ( !_nodeTypeMap.TryGetValue(typeName, out var targetType) )
                    throw new JsonException($"Unknown node type: {typeName }, The $type must be in ({string.Join(",",_nodeTypeMap.Keys)})");
               
            try
            {
                return (QueryNode)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\n{root.GetRawText()}",ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, QueryNode value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
    #endregion
     
}
