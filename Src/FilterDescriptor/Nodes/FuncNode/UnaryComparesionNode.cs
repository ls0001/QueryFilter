using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

#region 

/// <summary>
/// 一元函数节点
/// </summary>
public abstract class UnaryComparesionNode : CoreUnaryNode, IBooleanNode
{

    [JsonPropertyName("args")]
    public virtual QueryNode[] Arguments { get; set; }

    protected UnaryComparesionNode(QueryNodeType nodeType) : base(nodeType) { }

}

#endregion

#region 
/// <summary>
/// Opernd等于null比较节点
/// </summary>
public sealed class EqualNullNode : UnaryComparesionNode
{
    private static readonly ConstantNode<DBNull>[] _nullNode = [new ConstantNode<DBNull>()];

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public override QueryNode[] Arguments { get => _nullNode; set { return; } }

    public EqualNullNode() : base(QueryNodeType.eqnull) { }

    public EqualNullNode(QueryNode opernad) : this() { Operand = opernad; }

}

/// <summary>
/// 包含检查节点
/// </summary>
public sealed class InNode : UnaryComparesionNode
{
    [JsonPropertyName("args")]
    public override required QueryNode[] Arguments
    {
        get => base.Arguments;
        set => base.Arguments = (value?.Length >= 1) ? value : throw new ArgumentException($"The \"InNode\" must have at least one argument.");
    }

    public InNode() : base(QueryNodeType.@in) { }

    [SetsRequiredMembers]
    public InNode(QueryNode operand, QueryNode[] arguments) : this() { Operand = operand; Arguments = arguments; }
}

/// <summary>
/// 范围匹配节点
/// </summary>
public sealed class BetweenNode : UnaryComparesionNode
{
    [JsonPropertyName("args")]
    public override required QueryNode[] Arguments
    {
        get => base.Arguments;
        set => base.Arguments = (value?.Length >= 2) ? value : throw new ArgumentException($"The \"BetweenNode\" must have tow arguments. ");
    }

    public BetweenNode() : base(QueryNodeType.btw) { }

    [SetsRequiredMembers]
    public BetweenNode(QueryNode operand, QueryNode[] arguments) : this() { Operand = operand; Arguments = arguments; }
}


#endregion

