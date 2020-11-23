using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 条件表达式和操作符的配对
    /// </summary>
    /// <typeparam name="TOperator"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    //[DebuggerTypeProxy(typeof(QueryExprOperatorPairDebugView<,>))]
    [DebuggerDisplay("NodeOperatorPair: op={Operator} ; {Node}")]
    [JsonConverter(typeof(NodeOperatorJsonConverter))]
    public class NodeOperatorPair<TOperator, TNode>
        where TOperator : struct, Enum
        where TNode : QueryNode//,IEquatable<TNode>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal uint hashCode;

        [JsonPropertyName("op")]
        public TOperator Operator { get; set; }   // Flag of entry

        public TNode Node { get; set; }         // Value of entry

        public NodeOperatorPair() { }

        public NodeOperatorPair(TOperator op, TNode value)
        {
            Operator = op;
            Node = value;
        }

        public override int GetHashCode()
        {
            if (hashCode != 0)
                return (int)hashCode;
            if (Node != null)
                return HashCode.Combine(Node.GetHashCode(), Operator);
            return base.GetHashCode();
        }

        //public override string ToString()
        //{
        //    var sb = StringBuilderCache.GetInstance();
        //    sb.Append("{Operator:");

        //    if (Operator != null)
        //    {
        //        sb.Append(Operator);
        //    }
        //    sb.Append(",");
        //    sb.Append("Node:");

        //    if (Node != null)
        //    {
        //        sb.Append(Node);
        //    }

        //    sb.Append('}');

        //    return StringBuilderCache.GetString(sb);
        //}
    }


}
