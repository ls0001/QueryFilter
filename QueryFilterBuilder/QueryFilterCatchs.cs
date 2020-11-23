using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sag.Data.Common.Query
{

    internal static class QueryFilterCatchs
    {

        #region 属性表达式缓存

        /// <summary>
        /// int:类型的HashCode,string:属性名,Exception:字段或属性的表达式
        /// </summary>
        static Dictionary<int, Dictionary<string, MemberExpression>> _typeExprSet = new Dictionary<int, Dictionary<string, MemberExpression>>();
        static int _typesCount = 0;
        static int _maxTypesCount = 100;
        static int _minTypesCount = 50;
        static List<int> _typesForHotCache = new List<int>(_maxTypesCount);

        /// <summary>
        /// 获取或并添加属性表达式的缓存
        /// </summary>
        /// <param name="objExpr"></param>
        /// <param name="ptyName"></param>
        /// <returns></returns>
        public static MemberExpression GetPropertyOrFiledExprCatched(Expression objExpr, string ptyName)
        {
            bool hasTypeDic;
            var propertyName = ptyName;
            var typeHashCode = objExpr.Type.GetHashCode();
            hasTypeDic = _typeExprSet.TryGetValue(typeHashCode, out var propDict);    //先查找类型集合
            lock (_typesForHotCache)
            {
                if (hasTypeDic)
                {
                    //处理热缓存//把找到的缓存排在最新位置
                    _typesForHotCache.RemoveAt(_typesForHotCache.IndexOf(typeHashCode));
                    _typesForHotCache.Add(typeHashCode);
                    if (propDict.TryGetValue(propertyName, out var outExpr))   //拿到类型缓存，再拿属性表达式缓存
                        return outExpr;
                }
                else//如果无类型缓存
                {
                    if (_typesCount + 1 > _maxTypesCount)                             //如果超过缓存计数限制，移出最早加入的，剩下最小保持的数量
                    {
                        var removeTypesCount = _maxTypesCount - _minTypesCount;
                        for (int i = 0; i < removeTypesCount; i++)
                        {
                            if (_typeExprSet.Remove(_typesForHotCache[0]))
                            {
                                _typesCount--;
                            }
                            _typesForHotCache.RemoveAt(0);
                        }
                    }
                    propDict = new Dictionary<string, MemberExpression>();
                    hasTypeDic = _typeExprSet.TryAdd(typeHashCode, propDict);
                    if (hasTypeDic)
                    {
                        _typesForHotCache.Add(typeHashCode);
                        _typesCount++;
                    }
                }
                var propExpr = Expression.PropertyOrField(objExpr, ptyName);
                if (hasTypeDic && propDict.TryAdd(propertyName, propExpr))
                {
                    return propExpr;
                }
            }
            return null;
        }

        #endregion  //属性表达式缓存


        #region 被Expression.Call 调用的方法缓存
        /// <summary>
        /// 被Expression.Call 调用的方法缓存
        /// </summary>
        static MethodCatchedCollection _MethodSet = new MethodCatchedCollection();

        public static MethodInfo GetMethodCached(Type fromType, CallMethodId methodId, Type[] argument)
        {
            return _MethodSet.GetAndCacheMethod(fromType, methodId, argument);
        }


        public static MethodInfo GetMethodCached(Type fromType, string methodName, Type[] argument)
        {
            return _MethodSet.GetAndCacheMethod(fromType, methodName, argument);
        }

        internal sealed class MethodCatchedCollection
        {
            //Dictionary<Type, Dictionary<CallMethodId, DistinctSet<MethodInfo>>>
            //    types = new Dictionary<Type, Dictionary<CallMethodId, DistinctSet<MethodInfo>>>();

            //public MethodInfo GetMethod(Type fromType, CallMethodId methodId, Type[] argument)
            //{
            //    Dictionary<CallMethodId, DistinctSet<MethodInfo>> methods;
            //    DistinctSet<MethodInfo> set;
            //    Type type = fromType;
            //    #region
            //    //switch (methodId)
            //    //{
            //    //    case CallMethodId.ListContains:
            //    //    case CallMethodId.ListAddItem:
            //    //        {
            //    //            //type = typeof(Enumerable);
            //    //            if (type.FindType(typeof(ICollection)) != null)
            //    //                type = typeof(ICollection<>).MakeGenericType(type.GetGenericArguments());
            //    //            else
            //    //            if (type.IsAssignableFrom(typeof(IEnumerable)))
            //    //            {
            //    //                var elmtType = type.GetGenericArguments();
            //    //                type = typeof(Enumerable);
            //    //                foreach(var m in type.GetMethods())
            //    //                {
            //    //                    if (m.Name != "Contains") continue;
            //    //                    if (m.GetParameters().Length == 2)
            //    //                    {
            //    //                        var pis = m.GetParameters().ToArray() ;
            //    //                        argument = new Type[pis.Length];
            //    //                        for (var k = 0; k < argument.Length; k++) argument[k] = pis[k].ParameterType;
            //    //                        break;
            //    //                    }
            //    //                }

            //    //            }
            //    //            break;
            //    //        }

            //    //    case CallMethodId.StringContains:
            //    //    case CallMethodId.StringEndsWith:
            //    //    case CallMethodId.StringStartsWith:
            //    //    case CallMethodId.StringIsNullOrEmpty:
            //    //    case CallMethodId.StringCompare:
            //    //        {
            //    //            type = typeof(string);
            //    //            break;
            //    //        }
            //    //    case CallMethodId.ValueTypeCompareTo:
            //    //        {
            //    //            type = typeof(IComparable);
            //    //            break;
            //    //        }

            //    //}
            //    #endregion

            //    if (types.TryGetValue(type, out methods))
            //    {
            //        if (methods.TryGetValue(methodId, out set))
            //        {
            //            int argLength = argument.Length;
            //            var catchedMethods = set.ToArray();
            //            for(int i=0;i<catchedMethods.Length;i++)
            //            {
            //                var m = catchedMethods[i];
            //                var ps = m.GetParameters();
            //                var psLength = ps.Length;
            //                if (psLength == argLength)
            //                {
            //                    int j;
            //                    for (j = 0; j < psLength; j++)
            //                        if (ps[j].ParameterType != argument[j]) break;
            //                    if (j == psLength)
            //                        return m;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        set = new DistinctSet<MethodInfo>();
            //        methods = new Dictionary<CallMethodId, DistinctSet<MethodInfo>>();
            //        methods.Add(methodId, set);
            //        types.TryAdd(type, methods);
            //    }

            //    string methodName = null;
            //    switch (methodId)
            //    {
            //        case CallMethodId.ListContains:
            //            {
            //                //type = typeof(IList);
            //                methodName = "Contains";
            //                break;
            //            }
            //        case CallMethodId.ListAddItem:
            //            {
            //                //type = typeof(List<>);
            //                methodName = "Add";
            //                break;
            //            }
            //        case CallMethodId.StringContains:
            //            {
            //                //type = typeof(string);
            //                methodName = "Contains";
            //                break;
            //            }
            //        case CallMethodId.StringEndsWith:
            //            {
            //                //type = typeof(string);
            //                methodName = "EndsWith";
            //                break;
            //            }
            //        case CallMethodId.StringStartsWith:
            //            {
            //                //type = typeof(string);
            //                methodName = "StartsWith";
            //                break;
            //            }
            //        case CallMethodId.StringIsNullOrEmpty:
            //            {
            //                //type = typeof(string);
            //                methodName = "IsNullOrEmpty";
            //                break;
            //            }
            //        case CallMethodId.StringCompare:
            //            {
            //                //type = typeof(string);
            //                methodName = "Compare";
            //                break;
            //            }
            //        case CallMethodId.ValueTypeCompareTo:
            //            {
            //                methodName = "CompareTo";
            //                break;
            //            }

            //    }

            //    if (type == null || methodName == null) return null;
            //    var mi = type.GetMethod(methodName, argument);
            //    if (mi != null)
            //    {
            //        if (!(methods.TryGetValue(methodId, out set)))
            //            set = new DistinctSet<MethodInfo>();

            //        set.Add(mi);
            //        return mi;
            //    }
            //    return null;

            //}

            Dictionary<Type, Dictionary<CallMethodId, MethodInfo>>
                typesMethodsCache = new Dictionary<Type, Dictionary<CallMethodId, MethodInfo>>();

            public MethodInfo GetAndCacheMethod(Type fromType, CallMethodId methodId, Type[] argument)
            {
                Dictionary<CallMethodId, MethodInfo> methodDict;
                Type type = fromType;

                if (typesMethodsCache.TryGetValue(type, out methodDict))
                {
                    if (methodDict.TryGetValue(methodId, out var m))
                    {
                        return m;
                    }
                }

                string methodName = Units.GetCallMethodName(methodId);
                if (type == null || methodName == null) return null;
                var mi = type.GetMethod(methodName, argument);
                if (mi != null)
                {
                    if (methodDict == null)
                    {
                        methodDict = new Dictionary<CallMethodId, MethodInfo>();
                        typesMethodsCache.TryAdd(type, methodDict);
                    }
                    methodDict.Add(methodId, mi);
                    return mi;
                }
                return null;

            }

            public MethodInfo GetAndCacheMethod(Type fromType, string methodName, Type[] argument)
            {
                if (fromType == null || methodName == null) return null;

                Dictionary<CallMethodId, MethodInfo> methodDict;
                Type type = fromType;

                var methodId = Units.GetCallMethodId(type, methodName);
                if (typesMethodsCache.TryGetValue(type, out methodDict))
                {
                    if (methodDict.TryGetValue(methodId, out var m))
                    {
                        return m;
                    }
                }

                var mi = type.GetMethod(methodName, argument);
                if (mi != null)
                {
                    if (methodDict == null)
                    {
                        methodDict = new Dictionary<CallMethodId, MethodInfo>();
                        typesMethodsCache.TryAdd(type, methodDict);
                    }
                    methodDict.Add(methodId, mi);
                    return mi;
                }
                return null;

            }
        }//methodCatchodCollection

        #endregion //被Expression.Call 调用的方法缓存



    }//queryConditionCatch
}
