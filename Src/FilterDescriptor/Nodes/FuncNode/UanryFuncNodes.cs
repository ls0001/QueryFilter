using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;

 
/// <summary>
/// 一元函数节点
/// </summary>
public abstract class CoreUanryFuncNode : CoreUanryNode
{
    
    [JsonPropertyName("args")]
    public virtual required QueryNode[] Arguments { get; set; }
}

/// <summary>
/// 包含检查节点
/// </summary>
public sealed class InNode : CoreUanryFuncNode {
    public InNode()
    {
        base.NodeType = QueryNodeType.@in;
    }
}
 
/// <summary>
/// 范围匹配节点
/// </summary>
public sealed class BetweenNode : CoreUanryFuncNode
{
    [JsonPropertyName("args")]
    public override required QueryNode[] Arguments
    {
        get => base.Arguments;
        set => base.Arguments = (value?.Length >= 2) ? value:throw new ArgumentException($"The BetweenNode must have tow Arguments. "); 
    }

    public BetweenNode()
    {
        base.NodeType = QueryNodeType.btw;
    }
}


 
