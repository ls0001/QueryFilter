using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 常量值节点
/// </summary>
public sealed class ConstantNode<T> : QueryNode, ISingleValueNode,IConstantNode
{
    public  Type ValueType { get=>typeof(T);  }

    [MaybeNull]
    public T Value { get; set; }

    public ConstantNode() : base(QueryNodeType.val) { }

    public ConstantNode(T value):this() {Value = value; }
    

}



