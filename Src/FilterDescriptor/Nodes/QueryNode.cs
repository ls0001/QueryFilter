using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 查询节点基类（抽象类）
/// </summary>
public abstract class QueryNode : IQueryNode
{
    [property: NotNull]
    [JsonPropertyOrder(-99999)]
    [JsonPropertyName("$type")]
    public QueryNodeType NodeType { get; }

    protected QueryNode(QueryNodeType nodeType)
    {
        NodeType = nodeType;
    }

}

