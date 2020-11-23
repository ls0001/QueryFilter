using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 表示属性名或者属性值
    /// </summary>
    [DebuggerDisplay("NodeItem: Class[{ClassType}]; Name[{Name}]; TypeAs[{TypeAs?.ToString()}]")]
    [JsonConverter(typeof(PropertyItemJsonConverter))]
    public class PropertyItem : QueryItem //, IEquatable<NodeItem>
    {
        #region variable

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        string _name = null;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        Type _ClassType;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _hasCodeVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _lastHashCodeVersion = -1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _hashCodeCache;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        MemberInfo _memberInfoCache;



        #endregion //variable

        #region 属性


        /// <summary>
        /// 本项代表的字段(属性)名
        /// </summary>
        public string Name
        {
            get => _name;
            internal set
            {
                if (value == _name) return;
                _name = value;
                _hasCodeVersion++;
                _memberInfoCache = null;

            }
        }

        /// <summary>
        /// 获取或设置由其定义<inheritdoc cref="Name"/>的类型
        /// </summary>
        public Type ClassType 
        { 
            get => _ClassType; 
            internal set { _ClassType = value; _hasCodeVersion++; _memberInfoCache = null; } 
        }
 
        public override NodeItemFlag Flag => NodeItemFlag.Property;

        /// <summary>
        /// 获取<inheritdoc cref="Name"/>的数据类型（不含强制类型）
        /// </summary>
        /// <remarks>如果添加了<see cref="IItemFunction"/>则返回通过<seealso cref="QueryItem.Functions"/>计算的最终类型</remarks>
        public override Type DataType
        {
            get
            {
                if (Functions?.Length > 0) return Functions[^1].ReturnType;
                var inf = GetPropertyOrField();
                return inf.MemberType == MemberTypes.Property ? ((PropertyInfo)inf).PropertyType : ((FieldInfo)inf).FieldType;
            }
        }
     
        #endregion 属性

        #region 构造

        /// <summary>
        /// 使用指定类型和属性名称以及强制类型初始一个属性项
        /// </summary>
        /// <param name="classType">指示<paramref name="propertyName"/>所在的类型</param>
        /// <param name="propertyName">属性名(字段名)</param>
        /// <param name="typeAs">需要强制转换到的类型(默认=<see langword="null"/>,表示自动识别)</param>
        public PropertyItem(Type classType, string propertyName, Type typeAs) : base()
        {
            if (classType == null)
                throw new ArgumentNullException(MsgStrings.TypeCannotNull(ClassType?.FullName, ClassType));
            if (propertyName == null)
                throw new Exception(MsgStrings.PropertyNameCanotNull);
            //InnerKey = propertyName;
            ClassType = classType;
            _name = propertyName;

            TypeAs = typeAs;
        }

        public PropertyItem(Type propertyName, string name) : this(propertyName, name, null) { }

        internal PropertyItem() : base() { }

        #endregion 构造

        #region public methods
        public MemberInfo GetPropertyOrField()
        {
            if (_memberInfoCache != null) return _memberInfoCache;
            _memberInfoCache = ClassType.GetMember(_name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).FirstOrDefault();
            if (_memberInfoCache == null)
                throw new MemberAccessException(MsgStrings.MemberNotDefinedForType("", _name, ClassType));
            
            return _memberInfoCache;
        }


        public override int GetHashCode()
        {
            if (_name == null) return 0;
            if (_lastHashCodeVersion == _hasCodeVersion) return _hashCodeCache;

            while (true)
            {
                _lastHashCodeVersion = _hasCodeVersion;
                _hashCodeCache = CombineHashCode(ClassType.GetHashCode(), _name);
                _hashCodeCache = CombineHashCode(_hashCodeCache, ClassType.GetHashCode());
                if (_lastHashCodeVersion == _hasCodeVersion)
                    return _hashCodeCache;
            }
        }

        #endregion //public methods

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
