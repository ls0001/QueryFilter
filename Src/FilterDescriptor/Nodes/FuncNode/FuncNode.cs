using System.Text.Json.Serialization;

namespace DynamicQuery.Descriptor;
 
/// <summary>
/// 通用函数节点
/// </summary>
public sealed class FuncNode : CoreUanryFuncNode
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    ///// <summary>
    ///// 第一个参数是实例对象，后续参数是函数参数
    ///// </summary>
    //[JsonPropertyName("args")]
    //public required QueryNode[] Arguments { get; set; }

    public FuncNode()
    {
        base.NodeType = QueryNodeType.func;
    }
}

public sealed class SimpleFuncNode :QueryNode
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// 第一个参数是实例对象，后续参数是函数参数
    /// </summary>
    [JsonPropertyName("args")]
    public required QueryNode[] Arguments { get; set; }

    public SimpleFuncNode()
    {
        base.NodeType = QueryNodeType.sfunc;
    }
}
 

