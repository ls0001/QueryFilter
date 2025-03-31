using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

public abstract class CoreUnaryNode : QueryNode
{
    [JsonPropertyOrder(-10)]
    public virtual required QueryNode Operand { get; set; }

    public QueryNode OperandAlias
    {
        set => Operand = value;
    }

    protected CoreUnaryNode(QueryNodeType nodeType) : base(nodeType) { }
        
}
