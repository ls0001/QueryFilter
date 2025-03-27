using System.Collections.Immutable;
using System.Text.Json;


namespace DynamicQuery.Descriptor;

public sealed class NodeJsonConverterTypeRegistry
{
    private static readonly ImmutableDictionary<QueryNodeType, Type> _nodeTypeMap =
    ImmutableDictionary<QueryNodeType, Type>.Empty
    .AddRange(
    [
            new (QueryNodeType.and, typeof(AndAlsoNode)),
            new (QueryNodeType.or, typeof(OrElseNode)),
            new(QueryNodeType.eq, typeof(EqualNode)),
            new(QueryNodeType.neq, typeof(NotEqualNode)),
            new(QueryNodeType.lt, typeof(LessThanNode)),
            new(QueryNodeType.gt, typeof(GreaterThanNode)),
            new(QueryNodeType.lte, typeof(LessThanOrEqualNode)),
            new(QueryNodeType.gte, typeof(GreaterThanOrEqualNode)),
            new(QueryNodeType.add, typeof(AddNode)),
            new(QueryNodeType.sub, typeof(SubtractNode)),
            new(QueryNodeType.mul, typeof(MultiplyNode)),
            new(QueryNodeType.div, typeof(DivideNode)),
            new(QueryNodeType.mod, typeof(ModuloNode)),
            new(QueryNodeType.like, typeof(LikeNode)),
            new(QueryNodeType.btw, typeof(BetweenNode)),
            new(QueryNodeType.@in, typeof(InNode)),
            new(QueryNodeType.field, typeof(FieldNode)),
            new(QueryNodeType.val, typeof(ConstantNode)),
            new(QueryNodeType.value, typeof(ConstantNode)),
            new(QueryNodeType.not, typeof(NotNode)),
            new(QueryNodeType.eqnull, typeof(EqualNullNode)),
            new(QueryNodeType.eqn, typeof(EqualNullNode)),
            new(QueryNodeType.vfunc, typeof(ValueFuncNode)),
            new(QueryNodeType.bfunc, typeof(BoolFuncNode)),
            new(QueryNodeType.cfunc, typeof(CollectionFuncNode)),
            new(QueryNodeType.svfunc, typeof(SimpleValueFuncNode)),
            new(QueryNodeType.sbfunc, typeof(SimpleBoolFuncNode)),
            new(QueryNodeType.scfunc, typeof(SimpleCollectionFuncNode)),
            new(QueryNodeType.concat, typeof(ConcatNode)),
            new(QueryNodeType.pf, typeof(StartWithNode)),
            new(QueryNodeType.sf, typeof(EndWithNode)),
    ]);

    public static Type GetConcreteType(QueryNodeType type) =>
        _nodeTypeMap.TryGetValue(type, out var t)
            ? t
            : throw new KeyNotFoundException($"未注册的类型: {type}");

    public static QueryNodeType[] GetNodeTypeKeys() => _nodeTypeMap.Keys.ToArray();

    public static Type GetConcreteType(JsonElement jsonElement, JsonSerializerOptions options)
    {
        var root = jsonElement;

        if ( root.TryGetProperty("$type", out var typeProp) )
        {
            var nodeType = JsonSerializer.Deserialize<QueryNodeType>(typeProp.GetRawText(), options);
            return GetConcreteType(nodeType);
        }

        foreach ( var nodeType in _nodeTypeMap.Keys )
        {
            if ( root.TryGetProperty(nodeType.ToString(), out typeProp) && !string.IsNullOrWhiteSpace(typeProp.GetRawText()) )
            {
                return GetConcreteType(nodeType);
            }
        }
        throw new JsonException($"Missing required $type property \n {root.GetRawText()}");
    }

}
