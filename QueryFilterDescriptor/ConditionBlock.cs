using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{

    #region Class QueryConditionBlock

    /// <summary>
    /// 查询条件组,包含查询参数集合和查询块集合,这是一个树形结构,条件组内可以有独立的查询条件,也可以同时有子条件组,任意嵌套.
    /// 它可以表示这样的条件:field1="value1"  and (field2="value2" or (field3="value3" and field4="value4"))
    /// </summary>
    [JsonConverter(typeof(NodeBlockJsonConverter))]
    public class ConditionBlock : QueryBlock<ConditionItem, ConditionBlock, QueryLogical>
    {
        #region 构造

        /// <summary>
        /// 传入一个查询条件组,构造实例
        /// </summary>
        /// <param name="blocks">包含要加入的条件组的数组</param>
        /// <param name="logic">逻辑连接符,加入的各条件组之间逻辑关系</param>
        public ConditionBlock(QueryLogical logic, params ConditionBlock[] blocks) : this()
        {

            Add(logic, InsertionBehavior.Duplicates, blocks);

        }


        public ConditionBlock(string logicStr,params ConditionBlock[] blocks) : this()
        {
            // var logic = GetLogicFromString(logicStr);
            Add(logicStr, InsertionBehavior.Duplicates, blocks);

        }

        /// <summary>
        /// 传入一个查询条件数组,构造实例
        /// </summary>
        /// <param name="items">包含要添加的独立条件的数组</param>
        /// <param name="logic">逻辑连接符,加入的各条件之间的逻辑关系</param>
        public ConditionBlock(QueryLogical logic,params ConditionItem[] items) : this()
        {
            Add(logic, InsertionBehavior.Duplicates, items);
        }


        public ConditionBlock(string logicStr,params ConditionItem[] items)
        {
            Add(logicStr, InsertionBehavior.Duplicates, items);
        }

        /// <summary>
        /// 查询条件组构造函数
        /// </summary>
        public ConditionBlock()
        {
            ClassInitialize();
        }

        #endregion 构造

        #region private func
        private QueryLogical GetLogicFromString(string logicStr)
        {
            logicStr = logicStr.Trim().ToLower().Replace("  ", " ").Replace("  ", " ");
            return logicStr switch
            {
                "or" => QueryLogical.Or,
                "and" => QueryLogical.And,
                //"not" => QueryLogical.Not,
                //"any" => QueryLogical.Any,
                _ => QueryLogical.And,
            };
        }

        private void ClassInitialize()
        {
            this.GetOperatorWithString = GetLogicFromString;

        }

        #endregion private func

        //private void ClassTerminate()
        //{

        //}

        //~QueryParamBlock()
        //{
        //    ClassTerminate();
        //    //base.Finalize();
        //}

    }

    #endregion  //end region Class QueryConditionBlock

}
