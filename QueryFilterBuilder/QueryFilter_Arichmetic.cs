using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sag.Data.Common.Query
{
   public partial class QueryFilter
    {
        public static Expression MakeArichmeticAddExpr(Expression left, Expression right)
        {
            return BinaryExpression.Add(left, right);
        }
        public static Expression MakeArichmeticSubtractExpr(Expression left, Expression right)
        {
            return BinaryExpression.Subtract(left, right);
        }
        public static Expression MakeArichmeticMultiplyExpr(Expression left, Expression right)
        {
            return BinaryExpression.Multiply(left, right);
        }
        public static Expression MakeArichmeticDivideExpr(Expression left, Expression right)
        {
            return BinaryExpression.Divide(left, right);
        }
        public static Expression MakeArichmeticModuloExpr(Expression left, Expression right)
        {
            return BinaryExpression.Modulo(left, right);
        }
        public static Expression MakeArichmeticPowerExpr(Expression left, Expression right)
        {
            return BinaryExpression.Power(left, right);
        }
  
    }
}
