using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 二元节点基类
/// </summary>
public abstract class CoreBinaryNode : QueryNode
{
    public required QueryNode Left { get; set; }

    public virtual required QueryNode Right { get; set; }

    protected CoreBinaryNode(QueryNodeType nodeType) : base(nodeType) { }

}

