
using System.Diagnostics.CodeAnalysis;

namespace DynamicQuery.Descriptor;


public abstract class BinaryComparesionNode:CoreBinaryNode, IBooleanNode
{
    protected BinaryComparesionNode(QueryNodeType nodeType) : base(nodeType) { }

}

#region Specific Comparison Nodes

/// <summary>
/// 等于比较节点
/// </summary>
public sealed class EqualNode : BinaryComparesionNode
{
    public EqualNode():base(QueryNodeType.eq) { }

    [SetsRequiredMembers]
    public EqualNode(QueryNode left, QueryNode right) : this() { Left = left; Right = right; }

}

/// <summary>
/// 等于比较节点
/// </summary>
public sealed class NotEqualNode : BinaryComparesionNode
{
    public NotEqualNode():base(QueryNodeType.neq) { }

    [SetsRequiredMembers]
    public NotEqualNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 小于比较节点
/// </summary>
public sealed class LessThanNode : BinaryComparesionNode
{
    public LessThanNode():base(QueryNodeType.lt) { }

    [SetsRequiredMembers]
    public LessThanNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 大于比较节点
/// </summary>
public sealed class GreaterThanNode :BinaryComparesionNode
{
    public GreaterThanNode():base(QueryNodeType.gt) { }

    [SetsRequiredMembers]
    public GreaterThanNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 小于等于比较节点
/// </summary>
public sealed class LessThanOrEqualNode :BinaryComparesionNode
{
    public LessThanOrEqualNode():base(QueryNodeType.lte) { }

    [SetsRequiredMembers]
    public LessThanOrEqualNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 大于等于比较节点
/// </summary>
public sealed class GreaterThanOrEqualNode : BinaryComparesionNode
{
    public GreaterThanOrEqualNode():base(QueryNodeType.gte) { }

    [SetsRequiredMembers]
    public GreaterThanOrEqualNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

#endregion
