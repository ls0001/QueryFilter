using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor;

/// <summary>
/// 逻辑条件基节点
/// </summary>
public abstract class ConditionNode : QueryNode, IBoolResultNode
{
    [JsonPropertyName("body")]
    public required List<IBoolResultNode> Conditions { get; set; }

}

/// <summary>
/// Or逻辑条件节点
/// </summary>
public sealed class OrElseNode : ConditionNode
{
    public OrElseNode()
    {
        base.NodeType = QueryNodeType.or;
    }
}
/// <summary>
/// And逻辑条件节点
/// </summary>
public sealed class AndAlsoNode : ConditionNode
{
    
    public AndAlsoNode()
    {
        base.NodeType = QueryNodeType.and;
    }

}



