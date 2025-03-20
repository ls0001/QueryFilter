using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

public abstract class CoreStringFuncNode : CoreUanryNode
{

    [JsonPropertyName("pattern")]
    public required QueryNode Pattern { get; set; }

    [JsonPropertyName("comp")]
    public StringComparison? Comparison { get; set; }

}

public sealed class EndWithNode : CoreStringFuncNode
{
    public EndWithNode()
    {
        base.NodeType = QueryNodeType.sf;
    }
}

/// <summary>
/// 模糊匹配节点
/// </summary>
public sealed class LikeNode : CoreStringFuncNode
{
    public LikeNode()
    {
        base.NodeType = QueryNodeType.like;
    }
}


public sealed class StartWithNode : CoreStringFuncNode {
    public StartWithNode()
    {
        base.NodeType = QueryNodeType.pf;
    }
}

public sealed class ConcatNode : CoreBinaryNode {
    public ConcatNode()
    {
        base.NodeType = QueryNodeType.concat;
    }
}

