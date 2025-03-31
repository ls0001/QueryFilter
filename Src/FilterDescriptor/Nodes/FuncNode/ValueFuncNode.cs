using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

/// <summary>
/// 值函数节点基类
/// </summary>
public abstract class FuncNode : UnaryComparesionNode
{
    [JsonPropertyOrder(-200)]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    protected FuncNode(QueryNodeType type) : base(type) { }

}

/// <summary>
/// 值函数节点
/// </summary>
public sealed class ValueFuncNode : FuncNode, ISingleValueNode
{
    public ValueFuncNode() : base(QueryNodeType.vfunc) { }

    [SetsRequiredMembers]
    public ValueFuncNode(string funcName, QueryNode operand, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Operand = operand;
        Arguments = arguments;
    }

}

/// <summary>
/// Bool函数节点
/// </summary>
public sealed class BoolFuncNode : FuncNode, IBooleanNode
{
    public BoolFuncNode() : base(QueryNodeType.bfunc) { }

    [SetsRequiredMembers]
    public BoolFuncNode(string funcName, QueryNode operand, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Operand = operand;
        Arguments = arguments;
    }

}

/// <summary>
/// Bool函数节点
/// </summary>
public sealed class CollectionFuncNode : FuncNode, IMutipleValueNode
{
    public CollectionFuncNode() : base(QueryNodeType.cfunc) { }

    [SetsRequiredMembers]
    public CollectionFuncNode(string funcName, QueryNode operand, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Operand = operand;
        Arguments = arguments;
    }
}

/// <summary>
/// 通用函数节点基类
/// </summary>
public abstract class SimpleFuncNode : QueryNode
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// 第一个参数是实例对象，后续参数是函数参数
    /// </summary>
    [JsonPropertyName("args")]
    public required QueryNode[] Arguments { get; set; }

    public SimpleFuncNode(QueryNodeType nodeType) : base(nodeType) { }


}

/// <summary>
/// 通用的值函数节点
/// </summary>
public sealed class SimpleValueFuncNode : SimpleFuncNode, ISingleValueNode
{
    public SimpleValueFuncNode() : base(QueryNodeType.svfunc) { }

    [SetsRequiredMembers]
    public SimpleValueFuncNode(string funcName, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Arguments = arguments;
    }
}

/// <summary>
/// 通用的Bool函数节点
/// </summary>
public sealed class SimpleBoolFuncNode : SimpleFuncNode, IBooleanNode
{
    public SimpleBoolFuncNode() : base(QueryNodeType.sbfunc) { }

    [SetsRequiredMembers]
    public SimpleBoolFuncNode(string funcName, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Arguments = arguments;
    }
}

/// <summary>
/// 通用的集合函数节点
/// </summary>
public sealed class SimpleCollectionFuncNode : SimpleFuncNode, IMutipleValueNode
{
    public SimpleCollectionFuncNode() : base(QueryNodeType.scfunc) { }

    [SetsRequiredMembers]
    public  SimpleCollectionFuncNode(string funcName, QueryNode[] arguments) : this()
    {
        Name = funcName;
        Arguments = arguments;
    }
}
