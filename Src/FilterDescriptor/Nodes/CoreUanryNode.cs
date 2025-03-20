using System.Text.Json.Serialization;

namespace DynamicQuery. Descriptor;

public abstract class  CoreUanryNode : QueryNode
{
    [JsonPropertyName("operand")]
    public virtual required QueryNode Operand { get;set;}

    [JsonPropertyName("opd")]
    public QueryNode OperandAlias
    {
        set => Operand = value;
    }

}
