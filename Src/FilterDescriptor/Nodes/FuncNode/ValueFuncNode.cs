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
public sealed class ValueFuncNode : FuncNode,IValueResultNode
{
    public ValueFuncNode() : base(QueryNodeType.vfunc) { }
    
}

/// <summary>
/// Bool函数节点
/// </summary>
public sealed class BoolFuncNode : FuncNode,IBoolResultNode
{
    public BoolFuncNode() : base(QueryNodeType.bfunc) { }

}

/// <summary>
/// Bool函数节点
/// </summary>
public sealed class CollectionFuncNode : FuncNode,IValueCollectionNode
{
    public CollectionFuncNode() : base(QueryNodeType.cfunc) { }

}

/// <summary>
/// 通用函数节点基类
/// </summary>
public abstract class SimpleFuncNode :QueryNode
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
public sealed class SimpleValueFuncNode :SimpleFuncNode,IValueResultNode
{
 
    public SimpleValueFuncNode() : base(QueryNodeType.svfunc) { }

}

/// <summary>
/// 通用的Bool函数节点
/// </summary>
public sealed class SimpleBoolFuncNode :SimpleFuncNode, IBoolResultNode
{
    public SimpleBoolFuncNode() : base(QueryNodeType.sbfunc) { }

}

/// <summary>
/// 通用的集合函数节点
/// </summary>
public sealed class SimpleCollectionFuncNode:SimpleFuncNode, IValueCollectionNode
{
    public SimpleCollectionFuncNode() : base(QueryNodeType.scfunc) { }

}
