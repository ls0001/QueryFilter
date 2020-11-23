using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Sag.Data.Common.Query.Internal;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{

    #region QueryNode

    public abstract class QueryNode //: IEquatable<QueryNode>
    {
        #region variable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Type _typeAs = null;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal JsonSerializerOptions _jsonOptions;

        #endregion variable

        #region propertys 

        /// <summary>
        /// 获取或设置要强制类型转换到的指定类型,应用于块时默认值null,应用到属性或值时默认与其对应的值类型相同
        /// </summary>
        public Type TypeAs
        {
            get => _typeAs;
            set
            {
                //* if (value == null) throw new Exception(MsgStrings.ValueCannotNull);
                _typeAs = value;
            }
        }

        /// <summary>
        /// 获取属性字段的声明类型，或者字段值的实例类型
        /// </summary>
        public abstract Type DataType { get; }

        #endregion propertys

        #region Construct

        public QueryNode()
        {

        }

        #endregion

        #region json
        public virtual string ToJson(JsonSerializerOptions options = null)
        {
            var opt = options;
            if (opt != null)
            {
                if (!(opt.Converters.Any(o => o.GetType() == typeof(TypeToJsonConverter))))
                    opt.Converters.Add(new TypeToJsonConverter());
            }
            else
            {
                opt = GetJsonSerializerOptions();
            }
            return JsonSerializer.Serialize<object>(this, opt);
        }

        private JsonSerializerOptions GetJsonSerializerOptions()
        {
            if (_jsonOptions == null)
            {
                _jsonOptions = new JsonSerializerOptions();
                _jsonOptions.Converters.Add(new TypeToJsonConverter());
                _jsonOptions.IgnoreReadOnlyProperties = true;
                _jsonOptions.IgnoreNullValues = true;
#if DEBUG
                _jsonOptions.WriteIndented = true;
#endif
            }
            return _jsonOptions;
        }

        #endregion json

        #region hascode

        internal protected int ComputeHashCode(Type key) => key.GetHashCode();
        internal protected int ComputeHashCode(string key)
        {
            if (key == null)
                return 0;

            var ks = key.ToCharArray();
            var length = ks.Length;
            unchecked
            {
                int hashCode = length;
                for (int i = 0; i < length; i++)
                {
                    hashCode += (hashCode << 7) ^ ks[i];
                }

                // mix it a bit more
                hashCode -= hashCode >> 17;
                hashCode -= hashCode >> 11;
                hashCode -= hashCode >> 5;

                return hashCode;
            }

        }
        internal protected int CombineHashCode(int h1, object h2)
        {
            if (h2 == null) return h1;
                
            var key = h2;
            int hashCode = h1;
            int hk;
            if (key is int it)  //对于int 型，保持与直接调用CombineHasCode(int,int)的结果一致
                return CombineHashCode(h1, it);   
            else if (key is ValueType vt)
                hk = vt.GetHashCode();
            else if (key is string str)
                hk = ComputeHashCode(str);
            else if (key is Type tp)
                hk = ComputeHashCode(tp);
            else if (key is IList ks )
            {
                if (ks is IList<int> intlst)    //对于int[] 型，保持与直接调用CombineHashCode(int[])的结果一致
                {
                    var intarr = new int[intlst.Count + 1];
                    intarr[0] = h1;
                    intlst.CopyTo(intarr, 1);
                    return CombineHashCode(intarr);
                }

                var length = ks.Count;
                hk = 0;
                for (int i = 0; i < length; i++)
                {
                    var m = ks[i];
                    var mk = CombineHashCode(hk, m);
                    //hk += (hk << 7) ^ mk;
                    hk = mk;
                }
            }
            else if (key is IEnumerable kse)
            {
                if (kse is IEnumerable<int> intenum)
                {
                    var ints = intenum.ToArray();
                    var intarr = new int[ints.Length + 1];
                    intarr[0] = h1;
                    Array.Copy(ints, 0, intarr,1, ints.Length);
                    return CombineHashCode(intarr);  //对于int[] 型，保持与直接调用CombineHashCode(int[])的结果一致
                }
                hk = 0;
                foreach (var m in kse)  
                {
                    var mk = CombineHashCode(hk, m);
                    //hk += (hk << 7) ^ mk;
                    hk = mk;
                }
            }
            else
            { hk = key.GetHashCode(); }

            unchecked
            {
                hashCode += (hashCode << 7) ^ hk;
                // mix it a bit more
                hashCode -= hashCode >> 17;
                hashCode -= hashCode >> 11;
                hashCode -= hashCode >> 5;

                return hashCode;
            }
        }
        internal protected int CombineHashCode(params int[] key)
        {
            if (key == null || key.Length == 0)
                return 0;

            var ks = key;
            var length = ks.Length;
            unchecked
            {
                int hashCode = length;
                for (int i = 0; i < length; i++)
                {
                    hashCode += (hashCode << 7) ^ ks[i];
                }

                // mix it a bit more
                hashCode -= hashCode >> 17;
                hashCode -= hashCode >> 11;
                hashCode -= hashCode >> 5;

                return hashCode;
            }
        }
        public abstract override int GetHashCode();

        #endregion hascode

        //public abstract bool Equals([AllowNull] QueryNode other);


        //internal protected int ComputeHashCode(decimal key)
        //{
        //    return key.GetHashCode();
        //    //var ks = decimal.GetBits(key);
        //    //int hs = 4;
        //    //for (int i = 0; i < 4; i++)
        //    //{
        //    //    hs = CombineHashCode(hs, ks[i]);
        //    //}
        //    //return hs;
        //}
        //internal protected int ComputeHashCode(double key) => key.GetHashCode();
        //internal protected int ComputeHashCode(float key) => key.GetHashCode();
        //internal protected int ComputeHashCode(bool key) => ComputeHashCode(new byte[] { Convert.ToByte(key) });
        //internal protected int ComputeHashCode(byte key) => key.GetHashCode();
        //internal protected int ComputeHashCode(long key)
        //{
        //    return key.GetHashCode();
        //    //var ks = BitConverter.GetBytes(key);
        //    //var length = ks.Length;
        //    //unchecked
        //    //{
        //    //    int hashCode = length;
        //    //    for (int i = 0; i < length; i++)
        //    //    {
        //    //        hashCode += (hashCode << 7) ^ ks[i];
        //    //    }

        //    //    // mix it a bit more
        //    //    hashCode -= hashCode >> 17;
        //    //    hashCode -= hashCode >> 11;
        //    //    hashCode -= hashCode >> 5;

        //    //    return hashCode;
        //    //}
        //}

    }

    #endregion QueryNode

}