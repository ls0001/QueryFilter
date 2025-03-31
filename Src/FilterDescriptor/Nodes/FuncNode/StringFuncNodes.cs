using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

public abstract class CoreStringFuncNode : CoreUnaryNode
{

    [JsonPropertyName("pattern")]
    public required QueryNode Pattern { get; set; }

    [JsonPropertyName("comp")]
    public StringComparison? Comparison { get; set; }

    protected CoreStringFuncNode(QueryNodeType nodeType) : base(nodeType) { }


}

public sealed class EndWithNode : CoreStringFuncNode, IBooleanNode
{
    public EndWithNode() : base(QueryNodeType.sf) { }

    [SetsRequiredMembers]
    public EndWithNode(QueryNode operand,QueryNode pattern) : this() { Operand = operand; Pattern = pattern; }
}

/// <summary>
/// 模糊匹配节点
/// </summary>
public sealed class LikeNode : CoreStringFuncNode, IBooleanNode
{
    public LikeNode() : base(QueryNodeType.like) { }

    [SetsRequiredMembers]
    public LikeNode   (QueryNode operand, QueryNode pattern) : this() { Operand = operand; Pattern = pattern; }
}


public sealed class StartWithNode : CoreStringFuncNode, IBooleanNode
{
    public StartWithNode() : base(QueryNodeType.pf) { }

    [SetsRequiredMembers]
    public StartWithNode(QueryNode operand, QueryNode pattern) : this() { Operand = operand; Pattern = pattern; }
}


