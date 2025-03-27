
namespace DynamicQuery.Descriptor;


public abstract class BinaryComparesionNode:CoreBinaryNode, IBoolResultNode
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

}

/// <summary>
/// 等于比较节点
/// </summary>
public sealed class NotEqualNode : BinaryComparesionNode
{
    public NotEqualNode():base(QueryNodeType.neq) { }

}

/// <summary>
/// 小于比较节点
/// </summary>
public sealed class LessThanNode : BinaryComparesionNode
{
    public LessThanNode():base(QueryNodeType.lt) { }

}

/// <summary>
/// 大于比较节点
/// </summary>
public sealed class GreaterThanNode :BinaryComparesionNode
{
    public GreaterThanNode():base(QueryNodeType.gt) { }

}

/// <summary>
/// 小于等于比较节点
/// </summary>
public sealed class LessThanOrEqualNode :BinaryComparesionNode
{
    public LessThanOrEqualNode():base(QueryNodeType.lte) { }

}

/// <summary>
/// 大于等于比较节点
/// </summary>
public sealed class GreaterThanOrEqualNode : BinaryComparesionNode
{
    public GreaterThanOrEqualNode():base(QueryNodeType.gte) { }

}

#endregion
