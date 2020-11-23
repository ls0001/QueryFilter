using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    public partial class QueryFilter
    {
        #region 方法调用
        /// <summary>
        /// 通用方法调用
        /// </summary>
        /// <param name="coller">要对其使用方法的实例对象</param>
        /// <param name="methodId">方法名</param>
        /// <param name="argumentTypes">方法签名类型表</param>
        /// <param name="paramValues">传入参数</param>
        /// <returns></returns>

        public static Expression MakeMethodCallExpr(CallMethodId methodId, Type[] argumentTypes, Expression coller, params Expression[] paramValues)
        {
            var meth = GetMethod(coller.Type, methodId, argumentTypes);
            if (meth == null)
                throw new Exception(MsgStrings.MemberNotDefinedForType( _callMathNamesDict[methodId], coller.Type));
            return MakeMethodCallExpr(meth, coller, paramValues);
        }

        public static Expression MakeMethodCallExpr(string methodName, Type[] argumentTypes, Expression coller, params Expression[] paramValues)
        {
           return MakeMethodCallExpr(Units.GetCallMethodId(coller.Type, methodName), argumentTypes, coller, paramValues);
        }

        public static Expression MakeMethodCallExpr(MethodInfo method, Expression coller, params Expression[] paramValues)
        {
            if (method == null)
                throw new Exception(string.Format(MsgStrings.ValueCannotNull, nameof(method)));

            var meth = method;
            try
            {

                if (meth.IsStatic)
                {
                    if (method.GetParameters().Length > 1)
                    {
                        var newParamArr = new Expression[paramValues.Length + 1];
                        Array.Copy(paramValues, 0, newParamArr, 1, paramValues.Length);
                        newParamArr[0] = coller;
                        return Expression.Call(meth, newParamArr);
                    }
                    return Expression.Call(meth, coller);
                }
                else
                    return paramValues.Length > 0 ? Expression.Call(coller, meth, paramValues) : Expression.Call(coller, meth, paramValues);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 通用方法调用
        /// </summary>
        /// <param name="coller">要对其使用方法的实例对象</param>
        /// <param name="methodId">方法名</param>
        /// <param name="argumentTypes">方法签名类型表</param>
        /// <param name="paramValues">传入参数</param>
        /// <returns></returns>
        public static Expression MakeMethodCallExpr(CallMethodId methodId, Expression coller, params object[] paramValues)
        {
            Type[] argsTypes = new Type[paramValues.Length];
            for (int i = 0, x = paramValues.Length; i < x; i++)
            {
                argsTypes[i] = paramValues[i].GetType();
            }
            var meth = GetMethod(coller.Type, methodId, argsTypes);
            if (meth == null)
                throw new Exception(MsgStrings.MemberNotDefinedForType(_callMathNamesDict[methodId], coller?.Type));
            return MakeMethodCallExpr(meth, coller, paramValues);
        }

        public static Expression MakeMethodCallExpr(string methodName, Expression coller, params object[] paramValues)
        {
            return MakeMethodCallExpr(Units.GetCallMethodId(coller.Type, methodName), coller, paramValues);
        }

        private static Expression MakeMethodCallExpr(MethodInfo method, Expression coller, params object[] paramValues)
        {
            if (paramValues.Length > 0)
            {
                var pis = method.GetParameters();
                var args = new List<Expression>(paramValues.Length);
                for (int i = 0, x = paramValues.Length; i < x; i++)
                {
                    Type paramType = pis[i].ParameterType;
                    if (paramType.IsByRef)
                    {
                        paramType = paramType.GetElementType();
                    }
                    args.Add(Expression.Constant(paramValues[i], paramType));
                }
                return Expression.Call(coller, method, args);
            }
            return Expression.Call(coller, method);
        }

        public static MethodInfo GetMethod(Type fromType, CallMethodId methodId, params Type[] types)
        {
            return QueryFilterCatchs.GetMethodCached(fromType, methodId, types);

        }
        #endregion //方法调用

        /// <summary>
        /// 根据属性名称将属性包装成表达式
        /// </summary>
        /// <param name="entityExpr">包含指定属性的类的表达式</param>
        /// <param name="propertyName"></param>
        /// <returns>返回指定属性的表达式</returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="ArgumentNullException"/>
        public static Expression MakeProperyExpr(ParameterExpression entityExpr, string propertyName)
        {

            //var p = GetPropertyInfo(entityExpr, propertyName, true);
            //if (p == null)//如果查询的字段名错误或不存在
            //    throw new MemberAccessException(String.Format(MsgStrings.MemberNotDefinedForType, propertyName, entityExpr.Type.Name));
            //return Expression.Property(entityExpr, p);
            return QueryFilterCatchs.GetPropertyOrFiledExprCatched(entityExpr, propertyName);
        }
 

        public static Expression MakeListExpr(dynamic values, Type elementType)
        {
            Type valueType = values.GetType();
            int count;
            if (valueType.IsArray)
                count = values.Length;
            else
            if (valueType.FindType(typeof(ICollection)) != null)
                count = values.Count;
            else
                return null;


            //创建参数变量定义和赋值语句
            var parameters = new ParameterExpression[count];
            var varBlocks = new Expression[count + 1];
            for (int i = 0; i < count; i++)
            {
                parameters[i] = Expression.Parameter(elementType, "m_" + i.ToString());
                varBlocks[i] = Expression.Assign(parameters[i], Expression.Constant(values[i]));
            }

            #region 可以把这部分创建的表示list的表达式编译缓存起来

            //创建list实例和Add语句
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = Activator.CreateInstance(listType);
            var listExpr = Expression.Constant(list, listType);
            var typeArgs = new[] { elementType };
            var listAddMethod = GetMethod(listType, CallMethodId.ListAddItem, typeArgs);
            var listVar = Expression.Variable(listType);
            Expression[] block = new Expression[count + 2];
            block[0] = Expression.Assign(listVar, listExpr);
            for (int i = 0; i < count; i++)
            {
                block[i + 1] = Expression.Call(listVar, listAddMethod, parameters[i]);
            }
            block[count + 1] = listVar;
            //创建执行Add语句并返回list实例的块
            var listBlock = Expression.Block(new[] { listVar }, block);

            #endregion

            //把创建list的块设置为输出块的返回值语句(最后一句)
            varBlocks[count] = listBlock;

            //执行参数赋值语句,执行创建list的块,返回list实例
            return Expression.Block(parameters, varBlocks);
        }

        /// <summary>
        /// 根据属性名称获取属性，
        /// </summary>
        /// <param name="entity">包含要获取属性的类型表达式</param>
        /// <param name="ptyName">属性名称</param>
        /// <param name="hasNoPublic">是否查找非公开的属性</param>
        /// <returns>如果属性不存在或者名称错误，返回null</returns>
        /// <exception cref="ArgumentNullException"/>
        private static PropertyInfo GetPropertyInfo(ParameterExpression entity, string ptyName, bool hasNoPublic)
        {
            var typeOfEntity = entity.Type;
            var p = typeOfEntity.GetProperty(ptyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
            ?? (hasNoPublic ? typeOfEntity.GetProperty(ptyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy) : null);
            return p;
        }

    }

}

