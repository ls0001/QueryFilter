using System.Collections;
using System.Diagnostics.CodeAnalysis;


namespace DynamicQuery.Descriptor;

/// <summary>
/// 逻辑条件基节点
/// </summary>
//[JsonConverter(typeof(ConditionNodeJsonConverter))]
public abstract class LogicalNode : QueryNode, ILogicalNode, ICollection<IBooleanNode>
{
    public List<IBooleanNode> Body { get; set; } = new();

    private protected LogicalNode(QueryNodeType nodeType) : base(nodeType) { }


    #region ICollection impliment
    public int Count { get=>Body.Count; }

    public bool IsReadOnly { get=>false; }

    public void Add(IBooleanNode item)
    {
        if ( Body.Contains(item) ) return;
        Body.Add(item);
    }

    public void Clear()
    {
        Body.Clear();
    }

    public bool Contains(IBooleanNode item)
    {
        return Body.Contains(item);
    }

    public void CopyTo(IBooleanNode[] array, int arrayIndex)
    {
        Body.CopyTo(array, arrayIndex);
    }

    public bool Remove(IBooleanNode item)
    {
        return Body.Remove(item);
    }

    public IEnumerator<IBooleanNode> GetEnumerator()
    {
        return Body.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion ICollection Impl
}

public sealed class CondNode : LogicalNode
{
    public CondNode() : base(QueryNodeType.cond) { }

    [SetsRequiredMembers]
    public CondNode(List<ILogicalNode> body) : this() { Body =[ ..body]; }
}

/// <summary>
/// Or逻辑条件节点
/// </summary>
public sealed class OrNode : LogicalNode
{

    public OrNode() : base(QueryNodeType.or) { }

    [SetsRequiredMembers]
    public OrNode(List<IBooleanNode> body):this() {  Body=body; }
}

/// <summary>
/// And逻辑条件节点
/// </summary>
public sealed class AndNode : LogicalNode
{
    public AndNode() : base(QueryNodeType.and) { }

    [SetsRequiredMembers]
    public AndNode(List<IBooleanNode> body) : this() { Body = body; }
}



