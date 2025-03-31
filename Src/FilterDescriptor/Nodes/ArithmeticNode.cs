using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

#region Base Arithmetic Node
/// <summary>
/// 算术运算基类
/// </summary>
public abstract class ArithmeticNode : QueryNode, ISingleValueNode
{
    [JsonPropertyName("left")]
    public required QueryNode Left { get; set; }

    [JsonPropertyName("right")]
    public required QueryNode Right { get; set; }

    protected ArithmeticNode(QueryNodeType nodeType) : base(nodeType) { }

}
#endregion

#region Specific Arithmetic Nodes
/// <summary>
/// 加法节点
/// </summary>
public sealed class AddNode : ArithmeticNode
{
    public AddNode() : base(QueryNodeType.add) { }

    [SetsRequiredMembers]
    public AddNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 减法节点
/// </summary>
public sealed class SubtractNode : ArithmeticNode
{
    public SubtractNode() : base(QueryNodeType.sub) { }

    [SetsRequiredMembers]
    public SubtractNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 乘法节点
/// </summary>
public sealed class MultiplyNode : ArithmeticNode
{
    public MultiplyNode() : base(QueryNodeType.mul) { }

    [SetsRequiredMembers]
    public MultiplyNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 除法节点
/// </summary>
public sealed class DivideNode : ArithmeticNode
{
    public DivideNode() : base(QueryNodeType.div) { }

    [SetsRequiredMembers]
    public DivideNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 取模节点
/// </summary>
public sealed class ModuloNode : ArithmeticNode
{
    public ModuloNode() : base(QueryNodeType.mod) { }

    [SetsRequiredMembers]
    public ModuloNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

/// <summary>
/// 字符串相加
/// </summary>
public sealed class ConcatNode : ArithmeticNode
{
    public ConcatNode() : base(QueryNodeType.concat) { }

    [SetsRequiredMembers]
    public ConcatNode(QueryNode left, QueryNode right) : this() { base.Left = left; base.Right = right; }
}

#endregion
