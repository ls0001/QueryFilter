using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 字段访问节点
/// </summary>
public sealed class FieldNode : QueryNode, IValueResultNode
{
    [JsonPropertyName("path")]
    public required string Path { get; set; }

    public FieldNode() : base(QueryNodeType.field) { }

}



