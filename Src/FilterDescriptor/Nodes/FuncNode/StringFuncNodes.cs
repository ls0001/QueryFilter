using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

public abstract class CoreStringFuncNode : CoreUnaryNode
{

    [JsonPropertyName("pattern")]
    public required QueryNode Pattern { get; set; }

    [JsonPropertyName("comp")]
    public StringComparison? Comparison { get; set; }

}

public sealed class EndWithNode : CoreStringFuncNode, IBoolResultNode
{
    public EndWithNode()
    {
        base.NodeType = QueryNodeType.sf;
    }
}

/// <summary>
/// 模糊匹配节点
/// </summary>
public sealed class LikeNode : CoreStringFuncNode, IBoolResultNode
{
    public LikeNode()
    {
        base.NodeType = QueryNodeType.like;
    }
}


public sealed class StartWithNode : CoreStringFuncNode, IBoolResultNode
{
    public StartWithNode()
    {
        base.NodeType = QueryNodeType.pf;
    }
}


