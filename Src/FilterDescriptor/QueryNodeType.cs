namespace DynamicQuery;


public enum QueryNodeType
{
    and,
    or,
    eq,
    neq,
    lt,
    gt,
    lte,
    gte,
    add,
    sub,
    mul,
    div,
    mod,
    like,
    btw,
    @in,
    field,
    val,
    value,
    not,
    /// <summary>
    /// XX等于null表达式
    /// </summary>
    eqnull,
    eqn,
    /// <summary>
    /// 返回单个值的函数
    /// </summary>
    vfunc,
    /// <summary>
    /// 返回bool值的函数
    /// </summary>
    bfunc,
    /// <summary>
    /// 返回集合的函数
    /// </summary>
    cfunc,
    /// <summary>
    /// 返回单个值的通用函数
    /// </summary>
    svfunc,
    /// <summary>
    /// 返回bool值的通用函数
    /// </summary>
    sbfunc,
    /// <summary>
    /// 返回集合的通用函数
    /// </summary>
    scfunc,
    /// <summary>
    /// 字符拼接
    /// </summary>
    concat,
    /// <summary>
    /// 以前缀为开头
    /// </summary>
    pf,
    /// <summary>
    /// 以后缀为结尾
    /// </summary>
    sf,
}
