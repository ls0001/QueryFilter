using System;
using System.Collections;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 表示属性名或者属性值
    /// </summary>
    [DebuggerDisplay("NodeItem: Flag[{Flag}]; Value[{Value}]; TypeAs[{TypeAs?.ToString()}]")]
    [JsonConverter(typeof(NodeItemJsonConverter))]
    public class ValueItem : QueryItem //, IEquatable<ValueItem>
    {


        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        object _value = null;
        int _keyVersion = 0;
        int _valueVersion = 0;
        int _valueCacheVersion = -1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _lastKeyVersion = -1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _hashCodeCache;


        #region 构造

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">指示<see cref="value"/>是属性名称或者是属性值</param>
        /// <param name="value">属性名(字段名)或属性值</param>
        /// <param name="typeAs">需要强制转换到的类型(默认=<see langword="null"/>,表示与右值的Type相同)</param>
        public ValueItem(object value, Type typeAs) : base()
        {
            InnerKey = value;
            Value = value; //不是设置中间变量，而直接使用属性，目的是及时执行验证等关联动作。
            TypeAs = typeAs;
        }

        /// <summary>
        /// 初始化为节点值的项
        /// </summary>
        /// <param name="value">节点值</param>
        public ValueItem(object value) : this(value, null) { }


        public ValueItem() : base() { }

        #endregion 构造

        #region 属性

        /// <summary>
        /// 设置项的值，或者获取带类型转换的值。
        /// </summary>
        public object Value
        {
            get => GetValue();
            set
            {
                _keyVersion++;
                _valueVersion++;  //需要放在GetValue()之前，否则缓存不起作用
                InnerKey = value;
                _value = value;
                _value = GetValue();//放在这里，以便在设置Value时就进行验证。
                                    //  TypeAs ??=value?.GetType();
            }
        }

        /// <summary>
        /// 属性名或查询值,作为在块集合中时的键值组成
        /// </summary>
        protected virtual object InnerKey { get; set; }
        public override NodeItemFlag Flag { get => NodeItemFlag.Value; }

        /// <summary>
        /// 获取项值输出的类型
        /// </summary>
        public override Type DataType { get => TypeAs ?? _value?.GetType(); }


        #endregion 属性

        #region methods

        /// <summary>
        /// 获取项的值，如果已经设置了TypeAs则输出类型转换后的值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_valueVersion == _valueCacheVersion)
                return _value;
            _valueCacheVersion = _valueVersion;
            //转换为TypeAs
            return ChangeValueType(_value, TypeAs);
        }

        /// <summary>
        /// <inheritdoc cref="GetValue()"/>并再转换为指定类型
        /// </summary>
        /// <param name="toType">要转换到的类型</param>
        /// <returns></returns>
        public object GetValue(Type toType)
        {
            return ChangeValueType(GetValue(), toType);
        }


        private object ChangeValueType(object value, Type toType)
        {
            if (_value == null)
                return null;

            if (toType == null)
                return value;

            var valType = value.GetType();
            if (valType == toType)
                return value;

            if (valType == Constants.Static_TypeOfString || !(value is IEnumerable ev))
            {
                if (!TypeConverter.TryConvertSingleValueType(value, toType, out var objOut))
                    throw new InvalidCastException();
                return objOut;
            }

            if (value is ICollection cv)
            {
                if (!TypeConverter.TryConvertItemValuesType(cv, toType, out var cvOut))
                    throw new InvalidCastException();
                return cvOut;
            }

            if (TypeConverter.TryConvertItemValuesType(ev, toType, out var evOut))
                return evOut;

            //无法转换
            throw new InvalidCastException();
        }

        #endregion //methods

        public override int GetHashCode()
        {
            if (InnerKey == null) return 0;
            if (_lastKeyVersion == _keyVersion) return _hashCodeCache;

            while (true)
            {
                _lastKeyVersion = _keyVersion;
                _hashCodeCache = CombineHashCode(0, InnerKey);
                if (_lastKeyVersion == _keyVersion)
                    return _hashCodeCache;
            }
        }


        //public bool Equals([AllowNull] NodeItem other)
        //{
        //    return base.Equals(other);
        //}

        //public  bool Equals([AllowNull] QueryNode other)
        //{
        //    return base.Equals((NodeItem)other);
        //}

    }



}
