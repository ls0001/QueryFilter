using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sag.Data.Common.Query
{
    [DefaultProperty("Name")]
    [DebuggerDisplay("Name = {Name}")]
    //[Serializable]
    public class PropertyItem : QueryItem, IEquatable<PropertyItem>
    {
       
        #region 属性

        //public string Name { get => base.InternalKey.ToString(); set => base.InternalKey = value; }

        /// <summary>
        /// 获取或设置要强制类型转换到的指定类型,默认值null,表示与其对应的值类型相同
        /// </summary>
        public Type TypeAs { get; set; } = null;
        
        #endregion 属性


        #region 构造

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">属性名(字段名)</param>
        /// <param name="typeAs">需要强制转换到的类型(默认=<see langword="null"/>,表示与右值的Type相同)</param>
        public PropertyItem(string name, Type typeAs = null) : base()
        {
            TypeAs = typeAs;
        }

        public PropertyItem(string name) : base() { }

        public PropertyItem() : base() { }

        #endregion 构造


        public bool Equals([AllowNull] PropertyItem other)
        {
            return base.Equals(other);
        }

        //public override bool Equals([AllowNull] QueryNode other)
        //{
        //    return base.Equals((PropertyItem)other);
        //}

        public override int GetHashCode() => throw new NotImplementedException();
    }



}
