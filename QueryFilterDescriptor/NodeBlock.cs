using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{


    [JsonConverter(typeof(NodeBlockJsonConverter))]
    public class NodeBlock : QueryBlock<QueryItem, NodeBlock, QueryArithmetic> //, IEquatable<NodeBlock>
    {

        #region 构造

        public NodeBlock() : base() { }

        public NodeBlock(QueryArithmetic op, InsertionBehavior behavior, params NodeBlock[] block) : base(op, behavior, block) { }

        public NodeBlock(string opName, InsertionBehavior behavior, params NodeBlock[] block) : base(opName, behavior, block) { }

        public NodeBlock(QueryArithmetic op, InsertionBehavior behavior, params QueryItem[] items) : base(op, behavior, items) { }

        public NodeBlock(string opName, InsertionBehavior behavior, params QueryItem[] items) : base(opName, behavior, items) { }




        #endregion //构造

        //public bool Equals(NodeBlock other)
        //{
        //    return base.Equals(other);
        //}
        //public override bool Equals([AllowNull] QueryNode other)
        //{
        //    return base.Equals(other);
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        //public override bool Equals(QueryLeftBlock other)
        //{
        //    return base.Equals(other);
        //}



    }

    

}
