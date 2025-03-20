using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor;



/// <summary>
/// 逻辑条件节点
/// </summary>
public sealed class ConditionNode : QueryNode
{
    [JsonPropertyName("logic")]
    public string Logic { get; set; }

    [JsonPropertyName("body")]
    public List<QueryNode> Conditions { get; set; }

    public ConditionNode()
    {
        base.NodeType = QueryNodeType.cond;
    }

}



