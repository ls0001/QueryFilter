using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;


namespace DynamicQuery.Descriptor;

#region Core Interfaces
/// <summary>
/// 查询节点基础接口
/// </summary>
public interface IQueryNode
{
    public QueryNodeType NodeType { get; }
}
#endregion


