namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 查询比较操作
    /// </summary>
    /// <remarks></remarks>
    public enum QueryComparison
    {
        /// <summary>
        /// 等于（=）
        /// </summary>
        /// <remarks></remarks>
        Equal,
        /// <summary>
        /// 不等于(!=)
        /// </summary>
        /// <remarks></remarks>
        NotEqual,
        /// <summary>
        /// 小于
        /// </summary>
        /// <remarks></remarks>
        Less,
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <remarks></remarks>
        LessEqual,
        /// <summary>
        /// 大于
        /// </summary>
        /// <remarks></remarks>
        Greater,
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <remarks></remarks>
        GreaterEqual,
        /// <summary>
        /// 包含
        /// </summary>
        /// <remarks></remarks>
        Contains,
        /// <summary>
        /// 不包含
        /// </summary>
        /// <remarks></remarks>
        NotContains,
        /// <summary>
        /// 右匹配(右固定,左通配)包含
        /// </summary>
        /// <remarks></remarks>
        StartWith,
        /// <summary>
        /// 左匹配(左固定,右通配)包含
        /// </summary>
        /// <remarks></remarks>
        EndWith,
        /// <summary>
        /// 存在于清单里
        /// </summary>
        /// <remarks>被检索的集合必须实现了IList</remarks>
        InList,
        /// <summary>
        /// 不在清单里
        /// </summary>
        /// <remarks>被检索的集合必须实现了IList</remarks>
        NotInList,
        /// <summary>
        /// 介于之间
        /// </summary>
        Between,
        /// <summary>
        /// 为空
        /// </summary>
        /// <remarks></remarks>
        IsNullValue
    }

    /// <summary>
    /// 查询操作分组函数
    /// </summary>
    /// <remarks></remarks>
    public enum QueryGroupFunc
    {
        /// <summary>
        /// 求和
        /// </summary>
        /// <remarks></remarks>
        Sum, //（求和）
        /// <summary>
        /// 平均值
        /// </summary>
        /// <remarks></remarks>
        Avg, //（平均）
        /// <summary>
        /// 最小值
        /// </summary>
        /// <remarks></remarks>
        Min, //（最小值）
        /// <summary>
        /// 最大值
        /// </summary>
        /// <remarks></remarks>
        Max, //（最大值）
        /// <summary>
        /// 计数
        /// </summary>
        /// <remarks></remarks>
        Count, //（计数）
        /// <summary>
        /// 统计标准偏差
        /// </summary>
        /// <remarks></remarks>
        StDev, //（统计标准偏差）
        /// <summary>
        /// 统计方差
        /// </summary>
        /// <remarks></remarks>
        Var //（统计方差）。

    }

    /// <summary>
    /// 排序方向
    /// </summary>
    public enum QueryOrderByDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// 查询操作逻辑连接符
    /// </summary>
    /// <remarks></remarks>
    public enum QueryLogical
    {
        And,
        Or,
        /// <summary>
        /// 当非用于参数组的首个参数(组)时,将被自动更改为LgAnd.
        /// </summary>
        /// <remarks></remarks>
        //Null,
    }

    /// <summary>
    /// 查询的运算
    /// </summary>
    public enum QueryArithmetic
    {
        //Arithmetic
        /// <summary>
        /// 加法
        /// </summary>
        Add,
        /// <summary>
        /// 减法
        /// </summary>
        Sub,
        /// <summary>
        /// 乘法
        /// </summary>
        Mul,
        /// <summary>
        /// 除法
        /// </summary>
        Divide,
        /// <summary>
        /// 求余
        /// </summary>
        Mod,
        /// <summary>
        /// 幂
        /// </summary>
        Power,
        /// <summary>
        /// 条件选择 case when then else end   a==b ? c:d
        /// </summary>
        // Case
        ///<summary>
        ///类型转换
        ///</summary>
        TypeAs,
        ///<summary>
        ///默认值=0:根据.net定义枚举类型的默认值=0, 
        ///当有多个枚举值=0时,.Net源码显示:它是根据Array.Sort(Array name,Array value)排序,
        ///选择出第一个=0的枚举返回,因此尽管值等效,但枚举名称可能会不同,需要注意
        ///</summary>
        //Null = 0,

    }

    /// <summary>
    /// 集合插入行为
    /// </summary>
    public enum InsertionBehavior
    {
        /// <summary>
        /// 覆盖已经存在
        /// </summary>
        Overwrite,
        /// <summary>
        /// 可重复
        /// </summary>
        Duplicates,
        /// <summary>
        /// 忽略已经存在
        /// </summary>
        IgnoreExists

    }

    /// <summary>
    /// 一个表示查询项为:<inheritdoc cref="Value"/>
    /// 或<inheritdoc cref="Property"/>
    /// 或<inheritdoc cref="Condition"/>
    /// 的标记
    /// </summary>
    public enum NodeItemFlag
    {
        /// <summary>
        /// 查询值
        /// </summary>
        Value,
        /// <summary>
        /// 字段(属性)
        /// </summary>
        Property,
        /// <summary>
        /// 条件项
        /// </summary>
        Condition
    }

    public enum TypeConvertStrategy
    {
        /// <summary>
        /// 无转换
        /// </summary>
        NoChange,
        /// <summary>
        /// 指定类型
        /// </summary>
        SpecifyType,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum QueryExistsHandling
    {
        /// <summary>
        /// exists (select top 1 1 from ...)
        /// </summary>
        Any,
        /// <summary>
        /// not (not exists (select top 1 1 from ...))
        /// </summary>
        All
    }
}
