using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Sag.Data.Common;

namespace Sag.Data.Common.Query
{
    public partial class QueryFilter
    {



        /// <summary>
        /// 转换函数,若不可转换,返回null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destType"></param>
        /// <returns>若类型相同，或
        /// <paramref name="destType"/>为<c>null</c>，则原值返回
        /// </returns>
        public static Expression MakeTypeConvertExpr(Expression source, Type destType)
        {
            if (source.Type == destType || destType == null)
                return source;
            if (source == null)
                return null;
            //TODO 应先检测是否能够类型.net内置转换
            //   if (property.Type.IsConvertableTo(destType))

            try
            {
                return Expression.Convert(source, destType);
            }
            catch (Exception ex)
            {
                if (!(ex is InvalidOperationException))
                    return null;
                try
                {
                    string methodName;
                    var toType = destType;
                    if (toType.IsPrimitive)
                        methodName = "To" + toType.Name;
                    else if (destType.IsNullableType(out var type) && type.IsPrimitive)
                    {
                        toType = type;
                        methodName = "To" + toType.Name;
                    }
                    else
                    {
                        methodName = "ToString";
                    }
                    MethodInfo converterMethod;
                    converterMethod = typeof(Convert).GetMethod(methodName, new[] { source.Type });
                    return Expression.Convert(source, toType, converterMethod);
                }
                catch
                {
#if DEBUG
                    throw new InvalidCastException
                        (
                        "[" + source.ToString() + "] "
                        + string.Format(MsgStrings.InvalidTypeConvert(source.Type, destType))
                        );
#endif
                    return null;
                }
            }
        }

        /// <summary>
        /// 一个返回1=0的表达式,用于查询 条件失败的返回表达式
        /// </summary>
        /// <returns></returns>
        public static Expression InternalErrQueryExpr()
        {
            return Expression.Equal(_trueConstantExpr, _falseConstantExpr);
        }


        #region 构造各种比较表达式

        /// <summary>
        /// 如果是数组,按inList执行,如果单值会被转为string执行
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression MakeContainsExpr(Expression left, Expression right)
        {
            var rightType = right.Type;

            //如果右值是集合
            if (rightType.IsArray || (rightType.GetInterfaces().Contains(typeof(ICollection))))
                return MakeInListExpr(left, right);

            //按字符串处理
            left = MakeTypeConvertExpr(left, _typeOfString);
            if (left == null)
                return InternalErrQueryExpr();

            right = MakeTypeConvertExpr(right, _typeOfString);
            if (right == null)
                return InternalErrQueryExpr();

            return MakeMethodCallExpr(CallMethodId.StringContains, new[] { _typeOfString, _stringComaprsionTppe }, left, right, _stringComparisonExpr);

        }

        public static Expression MakeNotContainsExpr(Expression left, Expression right)
        {
            return Expression.Not(MakeContainsExpr(left, right));
        }

        /// <summary>
        /// 构造in 查询
        /// </summary>
        /// <example><code>c=>new[] {"name1","name2"}.contains(c.name)</code></example> 
        /// <param name="left"></param>
        /// <param name="right">代表集合的Expression</param>
        /// <returns></returns>
        public static Expression MakeInListExpr(Expression left, Expression right)
        {
            var methodName = "Contains";
            try
            {
                var elementType = right.Type.GetCollectionElementType();

                //右值不是集合 无法用in比较
                if (elementType == right.Type)
                    return MakeEqualExpr(left, right);

                //元素类型不同,
                if (left.Type != elementType)
                {
                    return InternalErrQueryExpr();
                }

                return MakeMethodCallExpr(CallMethodId.ListContains, new[] { left.Type }, right, left);
            }
            catch (Exception ex)
            {
                throw new Exception(MsgStrings.MemberNotDefinedForType( methodName, right.Type) + "\n\r" + ex.Message);
                //return InternalErrQueryExpr();
                 
            }
        }

        public static Expression MakeNotInListExpr(Expression left, Expression right)
        {
            var inListQuery = MakeInListExpr(left, right);
            var errQyery = InternalErrQueryExpr();

            //如果构建InList查询失败
            if (inListQuery == errQyery)
                return errQyery;

            return Expression.Not(inListQuery);
        }

        public static Expression MakeEqualExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));

            if (left.Type == right.Type)
                return Expression.Equal(left, right);

            //返回失败查询
            return InternalErrQueryExpr();

        }

        public static Expression MakeNotEqualExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));


            if (left.Type == right.Type)
                return Expression.NotEqual(left, right);


            return InternalErrQueryExpr();

        }

        public static Expression MakeLessExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));

            if (left.Type == right.Type)
            {
                if (left.Type.IsValueType)
                    return Expression.LessThan(left, right);

                if (left.Type == _typeOfString)
                {
                    return Expression.LessThan(
                        MakeMethodCallExpr(CallMethodId.StringCompare, new[] { _typeOfString, _typeOfString, _stringComaprsionTppe }, left, right, _stringComparisonExpr),
                        Expression.Constant(0)
                    );
                }
            }
            return InternalErrQueryExpr();

        }

        public static Expression MakeLessEqualExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));

            if (left.Type == right.Type)
            {

                if (left.Type.IsValueType)
                    return Expression.LessThanOrEqual(left, right);

                if (left.Type == _typeOfString)
                {
                    return Expression.LessThanOrEqual(
                        MakeMethodCallExpr(CallMethodId.StringCompare, new[] { _typeOfString, _typeOfString, _stringComaprsionTppe }, left, right, _stringComparisonExpr),
                        Expression.Constant(0)
                    );
                }
            }
            return InternalErrQueryExpr();

        }

        public static Expression MakeGreaterExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));

            if (left.Type == right.Type)
            {

                if (left.Type.IsValueType)
                    return Expression.GreaterThan(left, right);

                if (left.Type == _typeOfString)
                {
                    return Expression.GreaterThan(
                        MakeMethodCallExpr(CallMethodId.StringCompare, new[] { _typeOfString, _typeOfString, _stringComaprsionTppe }, left, right, _stringComparisonExpr),
                        Expression.Constant(0)
                    );
                }
            }
            return InternalErrQueryExpr();
        }

        public static Expression MakeGreaterEqualExpr(Expression left, Expression right)
        {
            if (left.Type.IsEnum)
                left = Expression.Convert(left, Enum.GetUnderlyingType(left.Type));

            if (right.Type.IsEnum)
                right = Expression.Convert(right, Enum.GetUnderlyingType(right.Type));

            if (left.Type == right.Type)
            {

                if (left.Type.IsValueType)
                    return Expression.GreaterThanOrEqual(left, right);

                if (left.Type == _typeOfString)
                {
                    return Expression.GreaterThanOrEqual(
                        MakeMethodCallExpr(CallMethodId.StringCompare, new[] { _typeOfString, _typeOfString, _stringComaprsionTppe }, left, right, _stringComparisonExpr),
                        Expression.Constant(0)
                    );
                }
            }
            return InternalErrQueryExpr();
        }

        public static Expression MakeStartWithExpr(Expression left, Expression right)
        {
            if (left.Type == _typeOfString && right.Type == _typeOfString)
                return MakeMethodCallExpr(CallMethodId.StringStartsWith, new[] { _typeOfString }, left, right);

            if (left.Type != _typeOfString)
                left = MakeTypeConvertExpr(left, _typeOfString);
            if (right.Type != _typeOfString)
            {
                right = MakeTypeConvertExpr(right, _typeOfString);
            }
            if(left==null || right==null)
                return InternalErrQueryExpr();

            return MakeMethodCallExpr(CallMethodId.StringStartsWith, new[] { _typeOfString }, left, right);
        }

        public static Expression MakeEndWithExpr(Expression left, Expression right)
        {
            if (left.Type == _typeOfString && right.Type == _typeOfString)
                return MakeMethodCallExpr(CallMethodId.StringEndsWith, new[] { _typeOfString }, left, right);

            if (left.Type != _typeOfString)
                left = MakeTypeConvertExpr(left, _typeOfString);
            if (right.Type != _typeOfString)
            {
                right = MakeTypeConvertExpr(right, _typeOfString);
            }
            if (left == null || right == null)
                return InternalErrQueryExpr();

            return MakeMethodCallExpr(CallMethodId.StringEndsWith, new[] { _typeOfString }, left, right);
        }

        public static Expression MakeIsNullExpr(Expression left, Expression right)
        {

            if (left.Type == _typeOfString)
            {
                return MakeMethodCallExpr(CallMethodId.StringIsNullOrEmpty, new[] { _typeOfString }, left, null);
            }

            right = MakeParameterizeConstant(null, typeof(Nullable<>).MakeGenericType(left.Type));
            return MakeEqualExpr(left, right);

            //return Expression.Call(left, typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) }), right);

        }

        public static Expression MakeNotIsNullExpr(Expression left, Expression right)
        {
            if (left.Type == _typeOfString)
            {
                return Expression.Not(MakeMethodCallExpr(CallMethodId.StringIsNullOrEmpty, new[] { _typeOfString }, left, null));
            }
            right = MakeParameterizeConstant(null, typeof(Nullable<>).MakeGenericType(left.Type));
            return MakeNotEqualExpr(left, right);

        }

        #endregion //构造比较表达式





    }
}
