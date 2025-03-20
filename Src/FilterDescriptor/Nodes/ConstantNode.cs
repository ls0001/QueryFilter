using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor;
 
 
    /// <summary>
    /// 常量值节点
    /// </summary>
[JsonConverter(typeof(ConstNodeJsonConverter))]
public sealed class ConstantNode : QueryNode
{
    [JsonPropertyName("type")]
    public string TypeName { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("val")]
    public object Val
    {
        set => Value = value;
    }

    public ConstantNode()
    {
        base.NodeType = QueryNodeType.val;
    }

}

  
 
