using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor;



/// <summary>
/// 查询节点基类（抽象类）
/// </summary>
[JsonConverter(typeof(QueryNodeJsonConverter))]
public abstract class QueryNode : IQueryNode
{
   // protected static readonly JsonValueTypeConverter Converter = new JsonValueTypeConverter();

    [property:NotNull]
    [JsonPropertyName("$type")]
    public  QueryNodeType NodeType { get; init; }

}

