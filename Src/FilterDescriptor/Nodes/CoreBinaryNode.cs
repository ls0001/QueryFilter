using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 二元节点基类
/// </summary>
public abstract class CoreBinaryNode : QueryNode
{
    [JsonPropertyName("left")]
    public required QueryNode Left { get; set; }

    [JsonPropertyName("right")]
    public virtual required QueryNode Right { get; set; }

    protected CoreBinaryNode(QueryNodeType nodeType) : base(nodeType) { }

}

