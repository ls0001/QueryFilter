using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Sag.Data.Common.Query;

namespace Sag.Data.Common.Query
{


    public class PropertyBlock : QueryBlock<ValueItem, PropertyBlock, QueryArithmetic>
    {
        #region 构造

        public PropertyBlock() : base() { }

        public PropertyBlock(QueryArithmetic op,InsertionBehavior behavior,params PropertyBlock[] block) : base(op, behavior, block) { }

        public PropertyBlock(string opName, InsertionBehavior behavior, params PropertyBlock[] block) : base(opName, behavior, block) { }

        public PropertyBlock(QueryArithmetic op, InsertionBehavior behavior, params ValueItem[] items) : base(op, behavior, items) { }

        public PropertyBlock(string opName, InsertionBehavior behavior, params ValueItem[] items) : base(opName, behavior, items) { }


        #endregion //构造

        
        //public override bool Equals([AllowNull] QueryNode other)
        //{
        //    return base.Equals(other);
        //}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public override bool Equals(QueryLeftBlock other)
        //{
        //    return base.Equals(other);
        //}
    }

    

}
