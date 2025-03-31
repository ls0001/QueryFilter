using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 字段访问节点
/// </summary>
public sealed class FieldNode : QueryNode, ISingleValueNode
{
    public required string Path { get; set; }

    public FieldNode() : base(QueryNodeType.field) { }

    [SetsRequiredMembers]
    public FieldNode(string path) : this() {  Path = path; }
    

}



