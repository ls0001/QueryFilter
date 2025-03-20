namespace  DynamicQuery;

public enum QueryNodeType
{
    cond,
    eq,
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
    @in ,
    field,
    val,
    value,
    not,
    nan,
    func,
    sfunc,
    concat,
    pf,
    sf,
}
