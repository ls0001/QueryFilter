using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;
using Sag.Data.Common.Query.Internal.JsonConvert;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{

    #region ItemBase

    [JsonConverter(typeof(QueryItemJsonConverter))]
    public abstract class QueryItem : QueryNode, IEquatable<QueryItem>
    {

        #region variable

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        QueryEqualComparer<QueryItem> _equalComparer;

        private protected List<IItemFunction> _Functions; 


        #endregion variable

        #region 构造

        public QueryItem() { initClass(); }
        #endregion //构造


        #region Propertys

        /// <summary>
        /// 获取<inheritdoc cref="NodeItemFlag"/>
        /// </summary>
        /// <value></value> 
        public abstract NodeItemFlag Flag { get; }

        /// <summary>
        /// 获取将<inheritdoc cref="IItemFunction"/>数组
        /// </summary>
        ///<remarks>这些函数将依次对本项进行计算。</remarks>  
        public IItemFunction[] Functions { get => (_Functions ??= new List<IItemFunction>()).ToArray(); }

        #endregion //Propertys

        #region public methods
            
        /// <summary>
        /// 添加一个对本项进行独立计算的函数。
        /// </summary>
        /// <param name="func">一个可以适用于项的函数<seealso cref="IItemFunction"/></param>
        /// <returns>如果所指定的函数不适用于本项，则返回<see cref="false"/></returns>
        public QueryItem AddFunctions(IItemFunction func)
        {
            if (!func.IsCanInvokeBy(DataType))
                //return this;
                throw new InvalidOperationException();
            (_Functions ??= new List<IItemFunction>(1)).Add(func) ;
            return this;
            
        }

        public void ClearFunctions()
        {
            _Functions.Clear();
        }

        #endregion  //public methods

        #region inner methods


        /// <summary>
        /// 设置相等比较器实例
        /// </summary>        
        protected void SetEqualComparer(QueryEqualComparer<QueryItem> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(GetEqualComparer), MsgStrings.ValueCannotNull(""));
            _equalComparer = value as QueryEqualComparer<QueryItem>
                ?? throw new InvalidCastException(MsgStrings.InvalidTypeConvert(value.GetType(),typeof(QueryEqualComparer<QueryItem>)));
            
        }

        /// <summary>
        /// 获取相等比较器实例
        /// </summary>  
        protected IEqualityComparer<QueryItem> GetEqualComparer() => _equalComparer;


        #endregion  methods


        #region private methods

        private void initClass()
        {
            _equalComparer = new QueryEqualComparer<QueryItem>();
            //_equalComparer = new QueryEqualComparer<QueryItem>((x, y) =>
            //{
            //    //if (y is QueryItem obj)
            //    //{
            //    if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false; //任何一个为Null
            //    if (ReferenceEquals(x, y)) return true;  //是否同引用的相同实例

            //    return x.GetHashCode() == y.GetHashCode();
            //    //}
            //    //else return false;
            //});
        }

        #endregion //private methods


        #region 实现比较接口相关


        public abstract override int GetHashCode();

        public override bool Equals(object obj)
        {
            if (!(obj is QueryItem o)) return false;
            return Equals(o);
        }

        public bool Equals([AllowNull] QueryItem other)
        {
            //if (EqualComparer == null)
            //    EqualComparer = new QueryEqualComparer<TItem>(obj =>
            //  {
            //      if (ReferenceEquals(this, obj)) return true;
            //      if (ReferenceEquals(obj, null)) return false;
            //      //if (string.Compare(this.ItemBody, obj.ItemBody, true) == 0) return true;
            //      return this.GetHashCode() == obj.GetHashCode();
            //      //return false;
            //  });
            return _equalComparer.Equals(this, other);

        }



        public static bool operator ==(QueryItem x, object y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(QueryItem x, object y)
        {
            return !x.Equals(y);
        }

        //public static bool operator ==(object x, QueryItem y)
        //{
        //    return y.Equals(x);
        //}

        //public static bool operator !=(object x, QueryItem y)
        //{
        //    return !y.Equals(x);
        //}



        #endregion //实现比较接口相关


    }

    #endregion ItemBase

}