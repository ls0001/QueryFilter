using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sag.Data.Common.Query
{
   public partial class QueryFilter
    {
        public static Expression MakeLogicAndExpr(Expression left,Expression right)
        {
            return BinaryExpression.And(left,right);
        }
        public static Expression MakeLogicAndAlsoExpr(Expression left,Expression right)
        {
            return BinaryExpression.AndAlso(left,right);
        }
        public static Expression MakeLogicOrExpr(Expression left,Expression right)
        {
            return BinaryExpression.Or(left,right);
        }
        public static Expression MakeLogicOrElseExpr(Expression left,Expression right)
        {
            return BinaryExpression.OrElse(left,right);
        }


    }
}
