using System.Linq.Expressions;
using System.Text.Json.Serialization;



namespace DynamicQuery.Descriptor;
 
 

    /// <summary>
    /// 逻辑非节点
    /// </summary>
    [JsonConverter(typeof(IsNotNodeJsonConverter))]
    public sealed class NotNode : QueryNode
    {
        private QueryNode _body;
        [JsonPropertyName("body")]
        public  required  QueryNode Body { get=>_body; set=>_body=value??throw new ArgumentNullException($"{nameof(NotNode)}.{nameof(Body)} was nonull able" ); }

    public NotNode()
    {
        base.NodeType = QueryNodeType.not;
    }
}
