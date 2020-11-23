using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
//using System.Diagnostics.CodeAnalysis;

//Predicate
namespace Sag.Data.Common.Query
{
    #region class QueryNodeEqualComparer

    /// <summary>
    /// 查询项左值的相等比较器
    /// </summary>
    /// <typeparam name="T">要被比较的类型</typeparam>
    [DebuggerDisplay("{typeof(T).Name,nq}")]
    public class QueryEqualComparer<T> : IEqualityComparer<T>, IEqualityComparer //where T : IEquatable<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        Func<T, object, bool> _CompareFunc;

        public QueryEqualComparer(Func<T, object, bool> compar)
        {
            _CompareFunc = compar;
        }

        public QueryEqualComparer() { }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            return GetComparer()(x, y);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public new bool Equals(object x, object y)
        {
 

            var comp = GetComparer();
            if (x is T xt) return comp(xt, y);   //x是T
            if (y is T ) return false;    //x不是T,但y 是T
            throw new NotSupportedException();   //两个都不是T,放弃比较.

            /**/
            //if (x == y) return true;
            //if (x == null || y == null) return false;
            //if ((x is T) && (y is T)) return Equals((T)x, (T)y);
            /////错误的参数类型,不能比较
            //return false;
        }

        private Func<T,object,bool> GetComparer()
        {
            if (_CompareFunc == null)
            {
                _CompareFunc = (x, y) =>
                {
                    if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return true;  //同时为null
                    if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false; //只有其中一个为null
                    if (ReferenceEquals(x, y)) return true;  //是否同引用的相同实例
                    if (!(y is T yt)) return false;
                    return x.GetHashCode() == yt.GetHashCode();

                };
            }
            return _CompareFunc;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            if (obj is T o) return GetHashCode((o));
            ///错误的参数类型
            return obj.GetHashCode();
        }


    }

    #endregion QueryNodeEqualComparer

}