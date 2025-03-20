using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DynamicQuery.Descriptor;

namespace DynamicQuery.Descriptor;

 
#region Base Arithmetic Node
/// <summary>
/// 算术运算基类
/// </summary>
public abstract class ArithmeticNode : QueryNode
{
    [JsonPropertyName("left")]
    public required QueryNode Left { get; set; }

    [JsonPropertyName("right")]
    public required QueryNode  Right { get; set; }
     
}
#endregion

#region Specific Arithmetic Nodes
/// <summary>
/// 加法节点
/// </summary>
public sealed class AddNode : ArithmeticNode {
    public AddNode()
    {
        base.NodeType = QueryNodeType.add;
    }
}

/// <summary>
/// 减法节点
/// </summary>
public sealed class SubtractNode : ArithmeticNode   {
    public SubtractNode()
    {
        base.NodeType = QueryNodeType.sub;
    }
}

/// <summary>
/// 乘法节点
/// </summary>
public sealed class MultiplyNode : ArithmeticNode {
    public MultiplyNode()
    {
        base.NodeType = QueryNodeType.mul;
    }
}

/// <summary>
/// 除法节点
/// </summary>
public sealed class DivideNode : ArithmeticNode {
    public DivideNode()
    {
        base.NodeType = QueryNodeType.div;
    }
}

/// <summary>
/// 取模节点
/// </summary>
public sealed class ModuloNode : ArithmeticNode {
    public ModuloNode()
    {
        base.NodeType = QueryNodeType.mod;
    }
}

#endregion
