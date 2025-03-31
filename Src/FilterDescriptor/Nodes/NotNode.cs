namespace DynamicQuery.Descriptor;



/// <summary>
/// 逻辑非节点
/// </summary> 
public sealed class NotNode : QueryNode, IBooleanNode
{
    private QueryNode _body;

    public required QueryNode Body { get => _body; set => _body = value ?? throw new ArgumentNullException($"{nameof(NotNode)}.{nameof(Body)} was nonull able"); }

#nullable disable
    public NotNode() : base(QueryNodeType.not) {  }
#nullable restore

    public NotNode(QueryNode body) : this() { Body = body; }

}
