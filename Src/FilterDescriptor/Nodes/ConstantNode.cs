using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 常量值节点
/// </summary>
//[JsonConverter(typeof(ConstNodeJsonConverter))]
public sealed class ConstantNode : QueryNode, IValueResultNode
{

    [JsonPropertyName("type")]
    public required Type ValueType { get; init; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("val")]
    public object Val
    {
        set => Value = value;
    }

    public ConstantNode() : base(QueryNodeType.val) { }


}



