
namespace DynamicQuery.Descriptor;


#region Specific Comparison Nodes

/// <summary>
/// Opernd等于null比较节点
/// </summary>
public sealed class IsNullNode : CoreUanryNode {
    public IsNullNode()
    {
        base.NodeType = QueryNodeType.nan;
    }
}

/// <summary>
/// 等于比较节点
/// </summary>
public sealed class EqualNode : CoreBinaryNode {
    public EqualNode()
    {
        base.NodeType = QueryNodeType.eq;
    }
}

/// <summary>
/// 小于比较节点
/// </summary>
public sealed class LessThanNode : CoreBinaryNode {
    public LessThanNode()
    {
        base.NodeType = QueryNodeType.lt;
    }
}

/// <summary>
/// 大于比较节点
/// </summary>
public sealed class GreaterThanNode : CoreBinaryNode {
    public GreaterThanNode()
    {
        base.NodeType = QueryNodeType.gt;
    }
}

/// <summary>
/// 小于等于比较节点
/// </summary>
public sealed class LessThanOrEqualNode : CoreBinaryNode {
    public LessThanOrEqualNode()
    {
        base.NodeType = QueryNodeType.lte;
    }
}

/// <summary>
/// 大于等于比较节点
/// </summary>
public sealed class GreaterThanOrEqualNode : CoreBinaryNode {
    public GreaterThanOrEqualNode()
    {
        base.NodeType = QueryNodeType.gte;
    }
}
#endregion
