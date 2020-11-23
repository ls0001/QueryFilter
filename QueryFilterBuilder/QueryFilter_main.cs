using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Sag.Data.Common.Query;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel;

namespace Sag.Data.Common.Query
{
    //    class ExpressionCreator

    /// <summary>
    /// 动态生成 表达式树
    /// </summary>
    public partial class QueryFilter
    {
        /// <summary>
        /// 查询条件值为数组时的分隔符
        /// </summary>
        public static string _arraySplitLetter = ",";

        /// <summary>
        /// 模块变量,用于构建条件比较表达式
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>条件表达式委托</returns>
        public delegate Expression _ExpressionEventHandler(Expression left, Expression right);

        /// <summary>
        /// 根据指定的过滤条件设定对象(<see cref="ConditionBlock"/>:<paramref name="conditions"/>)构建出一个或一组过滤表达式,
        /// <example>例如: 
        /// <code>
        /// "(obj)=>(obj.a.StartWith("1") &amp;&amp; !obj.b.Contains(new[]{1,2,3}) || (obj.c + obj.d != 2 &amp;&amp;  obj.e &lt;= 3))"
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">包含被查询字段的对象类型定义,例如实体的类定义</typeparam>
        /// <param name="conditions">一个查询条件配置的<see cref="ConditionBlock">集合对象</see></param>
        /// <returns>如果构建结果为Null,返回Null</returns>
        /// <exception cref="InvalidCastException">查询值类型转换错误</exception>
        /// <exception cref="MemberAccessException">查询属性(字段)名称错误或不存在</exception>
        /// <exception cref="InvalidOperationException">错误的类型方法调用</exception>
        public static Expression<Func<T, bool>> CreateFilterExpression<T>(ConditionBlock conditions)
        {
            if (conditions == null || conditions.AllCount == 0)
                return null;

            var entityExpr = Expression.Parameter(typeof(T));

            var _filter = CreateConditionBlockFilter(conditions, entityExpr);//, typeOfEntity);
            if (_filter == null)
                return null;
            return Expression.Lambda<Func<T, bool>>(_filter, entityExpr);
        }

        //public static Expression<Func<bool>> CreateFilterExpression(QueryConditionBlock conditions, Type entityType)
        //{
        //    if (conditions == null || conditions.AllCount == 0)
        //        return null;

        //    var entityExpr = Expression.Parameter(entityType);

        //    var _filter = CreateConditionBlockFilter(conditions, entityExpr);//, typeOfEntity);
        //    if (_filter == null)
        //        return null;
        //    return Expression.Lambda<Func<bool>>(_filter, entityExpr);
        //}

        public static Expression CreateConditionBlockFilter(ConditionBlock block, ParameterExpression entityExpr)//, Type typeOfEntity)
        {
            if (block == null || block.AllCount == 0)
                return null;

            if (entityExpr == null)
                throw new Exception( MsgStrings.TypeCannotNull( "EntityExpression"));

            var t = entityExpr.Type;
            //先拼接本组内的独立条件
            var filter = CreateConditionItemsFilter(block, entityExpr);

            //再拼接组内的子条件组(递归)
            var subBlockPairs = block.Blocks;
            for (int i = 0; i < subBlockPairs.Length; i++)
            {
                var subBlock = subBlockPairs[i].Node;                //子组
                if (subBlock == null || subBlock.AllCount == 0)
                    continue;     //如果子组没有内容,跳过

                #region 取得子条件组的拼接 

                Expression subBlockFilter;
                //如果子组内还有组,递归子组
                if (subBlock.BlockCount > 0)
                    subBlockFilter = CreateConditionBlockFilter(subBlock, entityExpr);//, t);
                else
                    subBlockFilter = CreateConditionItemsFilter(subBlock, entityExpr);//, t);//若当前子组内没有子组,拼接它的独立条件


                #endregion //取得子条件组的拼接

                if (subBlockFilter == default)
                    continue;                   //如果当前子组拼接结果为Null,跳过

                //拼接到子组

                if (filter == default)//如果前面条件为空,则当前子组是第一个条件
                {
                    filter = subBlockFilter;
                    continue;
                }

                var lg = subBlockPairs[i].Operator;              //子组逻辑
                if (lg == QueryLogical.And)
                    filter = Expression.AndAlso(filter, subBlockFilter);
                else if (lg == QueryLogical.Or)
                    filter = Expression.OrElse(filter, subBlockFilter);
                else
                {
                    continue;
                }


            }

            return filter;
        }

        /// <summary>
        /// 拼接一个条件组内的独立条件表达式
        /// </summary>
        /// <param name="conditionBlock">包含要拼接条件的组</param>
        /// <param name="entityExpr">包含被查询属性的对象类型的表达式</param>
        /// <param name="typeOfEntity">包含被查询属性的对象的类型,例如实体类的类型</param>
        /// <returns></returns>
        public static Expression CreateConditionItemsFilter(ConditionBlock conditionBlock, ParameterExpression entityExpr)//, Type typeOfEntity)
        {
            if (conditionBlock == default || conditionBlock.AllCount == 0)
                return default;

            //if (typeOfEntity == null) throw new Exception(string.Format(MsgStrings.TypeCannotNull, "typeOfEntity"));
            var filter = default(Expression);
            var itemPairs = conditionBlock.Items;
            for (int i = 0; i < itemPairs.Length; i++)
            {
                var pair = itemPairs[i];
                var item = pair.Node;                 //条件
                var cp = pair.Node.Comparison;        //条件比较符 
                var lg = pair.Operator;               //查询逻辑符 
                var itemMatchType = item.DataType;    //已匹配的条件类型
                //表达式左值(字段) 
                var left = MakeAirthmeticBlockExpr(item.PropertyBlock, entityExpr);
                if (item.PropertyBlock.DataType != itemMatchType)
                    left = MakeTypeConvertExpr(left, itemMatchType);
                //表达式右值
                var right = MakeAirthmeticItemsExpr(item.ValueBlock, entityExpr);
                if (item.ValueBlock.DataType != itemMatchType)
                    right = MakeTypeConvertExpr(right, itemMatchType);
                //创建比较表达式
                var resultExpr = MakeComparisonExpr(left, right, cp);

                //条件 串接  
                if (filter == default)                                  //条件组的第一个条件,
                    filter = resultExpr;                                //不需要逻辑符,形如:(column1=value)
                else if (lg == QueryLogical.And)
                    filter = Expression.AndAlso(filter, resultExpr);    //形如:and column1=value;
                else if (lg == QueryLogical.Or)
                    filter = Expression.OrElse(filter, resultExpr);     //形如:or column1=value;
                else
                    continue;                                           //跳过
            }

            return filter;
        }

        /// <summary>
        /// 算术项表达式串联
        /// </summary>
        /// <param name="block"></param>
        /// <param name="entityExpr">包含被查询属性的对象类型的表达式</param>
        /// <returns></returns>
        public static Expression MakeAirthmeticBlockExpr(NodeBlock block, ParameterExpression entityExpr)
        {
            // var filter = default(Expression);
            if (block == null || block.AllCount == 0)
                return null;

            if (entityExpr == null)
                throw new Exception( MsgStrings.TypeCannotNull("EntityExpression"));

            var matchType = block.DataType;
            //先拼接本组内的独立项
            var filter = MakeAirthmeticItemsExpr(block, entityExpr);//, t);

            //再拼接组内的子组(递归)
            var subBlocksPair = block.Blocks;
            for (int i = 0; i < subBlocksPair.Length; i++)
            {
                var subBlock = subBlocksPair[i].Node;                //子组
                if (subBlock == null || subBlock.AllCount == 0)
                    continue;     //如果子组没有内容,跳过

                #region 取得子属性(字段)组的拼接 

                Expression subBlockFilter;// = default;                
                if (subBlock.BlockCount > 0)
                    subBlockFilter = MakeAirthmeticBlockExpr(subBlock, entityExpr);//如果子组内还有组,递归子组
                else
                    subBlockFilter = MakeAirthmeticItemsExpr(subBlock, entityExpr);//若当前子组内没有子组,拼接它的独立项


                #endregion //取得子组的拼接

                //如果当前子组拼接结果为Null,跳过
                if (subBlockFilter == default)
                    continue;

                //匹配类型
                if (subBlock.DataType != matchType)
                    subBlockFilter= MakeTypeConvertExpr(subBlockFilter, matchType);

                //拼接到子组
                if (filter == default)//如果前面为空,则当前子组是第一个
                {
                    filter = subBlockFilter;
                    continue;
                }
                else
                {
                    var lg = subBlocksPair[i].Operator;              //子组运算
                    if (lg == QueryArithmetic.Add)
                        filter = Expression.Add(filter, subBlockFilter);
                    else if (lg == QueryArithmetic.Sub)
                        filter = Expression.Subtract(filter, subBlockFilter);
                    else if (lg == QueryArithmetic.Mul)
                        filter = Expression.Multiply(filter, subBlockFilter);
                    else if (lg == QueryArithmetic.Divide)
                        filter = Expression.Divide(filter, subBlockFilter);
                    else if (lg == QueryArithmetic.Mod)
                        filter = Expression.Modulo(filter, subBlockFilter);
                    else
                        continue;
                }

            }

            return filter;

        }

        /// <summary>
        /// 构建并串接算术型Block中Items的表达式
        /// </summary>
        /// <param name="block"></param>
        /// <param name="entityExpr"></param>
        /// <returns></returns>
        public static Expression MakeAirthmeticItemsExpr(NodeBlock block, ParameterExpression entityExpr)
        {
            //if (typeOfEntity == null) throw new Exception(string.Format(MsgStrings.TypeCannotNull, "typeOfEntity"));       
            if (block == default || block.ItemCount == 0)
                return default;

            var matchType = block.DataType;  //匹配整个列表的类型            
            var filter = default(Expression);
            var itemPairs = block.Items;
            for (int i = 0; i < itemPairs.Length; i++)
            {
                var item = itemPairs[i].Node;
                var op = itemPairs[i].Operator;                 //运算符 
                var flag = item.Flag;
                var typeAs = item.TypeAs;

                //表达式
                Expression itemExpr = null;
                if (flag == NodeItemFlag.Property)  //如果是属性项
                {
                    var name = ((PropertyItem)item).Name;        //属性(字段)名
                    itemExpr = MakeProperyExpr(entityExpr, name); //If the property does not exist will  throw exception
                    //属性自身的函数
                    for (int f =0; f < item.Functions.Length;f++)
                    {
                        var func = item.Functions[f];
                        itemExpr = MakeMethodCallExpr(func.Name, itemExpr, func.ArgumentsValues);
                    }
                    //比较或并转换项本身的强制类型
                    if (typeAs != null && typeAs != itemExpr.Type)
                        itemExpr = MakeTypeConvertExpr(itemExpr, typeAs);
                    //比较或并转换列表的匹配类型
                    if (matchType != itemExpr.Type)
                        itemExpr = MakeTypeConvertExpr(itemExpr, matchType);
#if DEBUG
                    if (itemExpr == null)
                        throw new Exception(MsgStrings.InvalidPropertyExpressionConvert(name,typeAs,matchType));
#endif
                }
                else//如果是条件值项
                if (flag == NodeItemFlag.Value)
                {
                    var valItem = (ValueItem)item;
                    object value= valItem.Value;
                    if (valItem.DataType != matchType)
                        value =Convert.ChangeType(value,matchType);

                    itemExpr = MakeQueryValueExpr(value, matchType, null);
                }
#if !DEBUG
                if (itemExpr == null)
                    throw new Exception($"构建表达式错误：\n\r项类型：{item.Flag}，值：{value}\n\r，值类型{value.GetType()}\n\r目标类型：{itemsType}");    
#endif
                if (itemExpr == null)
                    continue;
                
                //如果是第一个属性(字段)
                if (filter == default)
                {
                    filter = itemExpr;
                    continue;
                }

                //前面的属性(字段)作左值
                Expression left = filter;
                //当前字段作为表达式右值
                var right = itemExpr;
                //属性(字段) 串接  
                //filter = MakeBinaryExpression(left, right, op) ?? filter;
                filter = op switch
                {
                    QueryArithmetic.Add => Expression.Add(left, right),
                    QueryArithmetic.Sub => Expression.Subtract(left, right),
                    QueryArithmetic.Mul => Expression.Multiply(left, right),
                    QueryArithmetic.Divide => Expression.Divide(left, right),
                    QueryArithmetic.Mod => Expression.Modulo(left, right),
                    QueryArithmetic.Power => Expression.Power(left, right),
                    _ => throw new NotSupportedException(MsgStrings.NotSupportedArithmeticException(left, right, op.ToString())),
                };

                //if (op == QueryArithmetic.Add)
                //    filter = Expression.Add(left, right);
                //else if (op == QueryArithmetic.Sub)
                //    filter = Expression.Subtract(left, right);    //形如:column1-column2;
                //else if (op == QueryArithmetic.Mul)
                //    filter = Expression.Multiply(left, right);
                //else if (op == QueryArithmetic.Divide)
                //    filter = Expression.Divide(left, right);
                //else if (op == QueryArithmetic.Mod)
                //    filter = Expression.Modulo(left, right);
                //else
                //    continue;                                           //跳过

            }
            return filter;
        }

        public static Expression MakeQueryValueExpr(object value, Type destType, string paramName)
        {
            if (value == null)
            {
                return MakeParameterizeConstant(null, typeof(object), paramName);

            }
            return Expression.Constant(value);

        }


        /// <summary>
        /// 构建一个参数化常量或变量
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">=value.GetType(),单独拿出来,方便处于循环中提高性能</param>
        /// <param name="paramName"></param>
        /// <returns>当执行块表达式时, 它将返回块中最后一个表达式的值</returns>
        public static Expression MakeParameterizeConstant(dynamic value, Type type, string paramName = null)
        {
            var varExpr = Expression.Variable(type, paramName);
            var assignExpr = Expression.Assign(varExpr, Expression.Constant(value, type));
            var blockExpr = Expression.Block(new ParameterExpression[] { varExpr }, assignExpr, varExpr);
            return blockExpr;
        }

        /// <summary>
        /// 根据比较符和左右值创建一个比较表达式
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static Expression MakeComparisonExpr(Expression left, Expression right, QueryComparison cp)
        {
            _ExpressionEventHandler exprHandler = default(_ExpressionEventHandler);

            exprHandler = cp switch
            {
                QueryComparison.Equal
                => MakeEqualExpr,
                QueryComparison.NotEqual
                => MakeNotEqualExpr,
                QueryComparison.Contains
                => MakeContainsExpr,
                QueryComparison.NotContains
                => MakeNotContainsExpr,
                QueryComparison.Greater
                => MakeGreaterExpr,
                QueryComparison.GreaterEqual
                => MakeGreaterEqualExpr,
                QueryComparison.Less
                => MakeLessExpr,
                QueryComparison.LessEqual
                => MakeLessEqualExpr,
                QueryComparison.StartWith
                => MakeStartWithExpr,
                QueryComparison.EndWith
                => MakeEndWithExpr,
                QueryComparison.InList
                => MakeInListExpr,
                QueryComparison.NotInList
                => MakeNotInListExpr,
                QueryComparison.IsNullValue
                => MakeIsNullExpr,

                _ => MakeEqualExpr,
            };

            Expression resultExpr = exprHandler(left, right);
            return resultExpr;
        }

        public static Expression MakeLogicExpr(Expression left, Expression right, QueryLogical lg)
        {
            _ExpressionEventHandler exprHandler = default(_ExpressionEventHandler);

            exprHandler = lg switch
            {
                QueryLogical.And
                => MakeLogicAndAlsoExpr,
                QueryLogical.Or
                => MakeLogicOrElseExpr,
                //QueryLogical.Not
                //=> makenot,
                //QueryLogical.Any
                //=> MakeContainsExpr,
                _ => MakeLogicAndAlsoExpr,
            };

            Expression resultExpr = exprHandler(left, right);
            return resultExpr;
        }

        public static Expression MakeArithmethExpr(Expression left, Expression right, QueryArithmetic am)
        {
            _ExpressionEventHandler exprHandler = default(_ExpressionEventHandler);

            exprHandler = am switch
            {
                QueryArithmetic.Add
                => MakeArichmeticAddExpr,
                QueryArithmetic.Sub
                => MakeArichmeticSubtractExpr,
                QueryArithmetic.Mul
                => MakeArichmeticMultiplyExpr,
                QueryArithmetic.Divide
                => MakeArichmeticDivideExpr,
                QueryArithmetic.Mod
                => MakeArichmeticModuloExpr,
                QueryArithmetic.Power
                => MakeArichmeticPowerExpr,
                _ => null,
            };

            Expression resultExpr = exprHandler(left, right);
            return resultExpr;
        }

        public static Expression MakeBinaryExpression(Expression left, Expression right, QueryComparison cp)
        {
            return MakeComparisonExpr(left, right, cp);
        }

        public static Expression MakeBinaryExpression(Expression left, Expression right, QueryLogical lg)
        {
            return MakeLogicExpr(left, right, lg);
        }

        public static Expression MakeBinaryExpression(Expression left, Expression right, QueryArithmetic am)
        {
            return MakeArithmethExpr(left, right, am);
        }

        //protected override Expression VisitConstant(ConstantExpression node)
        //{
        //    Type tupleType;
        //    ParameterizableTypes.TryGetValue(node.Type, out tupleType);
        //    if (ParameterizableTypes.TryGetValue(node.Type, out tupleType))
        //    {
        //        //Replace the ConstantExpression to PropertyExpression of Turple<T>.Item1
        //        //Entity Framework 5 will parameterize the expression when the expression tree is compiled
        //        Object wrappedObject = Activator.CreateInstance(tupleType, new[] { node.Value });
        //        Expression visitedExpression = Expression.Property(Expression.Constant(wrappedObject), "Item1");
        //        return visitedExpression;
        //    }
        //    return base.VisitConstant(node);
        //}


    }
}

////使用方法
//SagExpression CE = new SagExpression();
//var resultList = db.EntityClass.Where(CE.Equal<EntityClass>("FieldName1,FieldName2", Value1,Value2)).ToList();
