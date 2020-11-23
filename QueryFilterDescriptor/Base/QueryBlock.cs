using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sag.Data.Common.Query.Internal;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{


    #region blockbase

    //[DebuggerTypeProxy(typeof(QueryBlockDebugView<,,>))]
    [DebuggerDisplay("QueryBlock:Items[{ItemCount}],Blocks[{BlockCount}] <{typeof(TItem).Name,nq},{typeof(TBlock).Name,nq},{typeof(TOpEnum).Name,nq}>")]
    public abstract class QueryBlock<TItem, TBlock, TOpEnum> : QueryNode, IEquatable<QueryBlock<TItem, TBlock, TOpEnum>>
        where TItem : QueryItem//, IEquatable<TItem>
        where TBlock : QueryBlock<TItem, TBlock, TOpEnum>//,IEquatable<QueryBlock<TItem,TBlock,TOpEnum>>
        where TOpEnum : struct, Enum
    {
        #region 变量

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NodeCollection<TOpEnum, TItem> m_items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NodeCollection<TOpEnum, TBlock> m_blocks;

        /// <summary>
        /// 子块列表整体的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type _matchedSubBlocksDataType = null;

        /// <summary>
        /// 项与项之间，项与子块整集之间的匹配类型，即代表本块的整体类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type _matchedDataType = null;

        private Type _matchedItemsType = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _blockVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _itemVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _hashCodeVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _lastHashCodeVersion = -1;

        private int _hashCodeCache = 0;

        const InsertionBehavior defaultItemInsertBehavior = InsertionBehavior.Duplicates;
        const InsertionBehavior defaultBlockInsertBehavvior = InsertionBehavior.IgnoreExists;

        #endregion //变量

        #region 构造

        /// <summary>
        /// 传入一个查询块,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,与块内其它子块或项之间逻辑关系</param>
        /// <param name="behavior">是否阻止添加具有相同逻辑连接符的重复的块</param>
        /// <param name="blocks">包含要加入的块的数组</param>
        public QueryBlock(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks) : this()
        {
            AddBlock(op, behavior, blocks);
        }

        /// <summary>
        /// 传入一个查询块数组,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,指示与块与块之间,项与项之间,项与块之间的逻辑关系</param>
        /// <param name="behavior">指示是否重复的行为</param>
        /// <param name="itemArray">包含要添加的块的数组</param>
        public QueryBlock(string opName, InsertionBehavior behavior, params TBlock[] blocks) : this()
        {
            AddBlock(opName, behavior, blocks);

        }

        /// <summary>
        /// 传入一个查询项数组,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,指示与同一块内其它项或子块之间的逻辑关系</param>
        /// <param name="behavior">指示条件之间是否重复的行为</param>
        /// <param name="itemArray">包含要添加的项的数组</param>
        public QueryBlock(TOpEnum op, InsertionBehavior behavior, params TItem[] itemArray) : this()
        {
            AddItem(op, behavior, itemArray);
        }

        public QueryBlock(string opName, InsertionBehavior behavior, params TItem[] itemArray)
        {
            AddItem(opName, behavior, itemArray);
        }

        /// <summary>
        /// 查询块构造函数
        /// </summary>
        public QueryBlock()
        {
            ClassInitialize();

        }

        #endregion 构造

        #region Add

        public int Add(TOpEnum op, InsertionBehavior behavior, params TItem[] items)
        {
            return AddItem(op, behavior, items);
        }

        public int Add(string opName, InsertionBehavior behavior, params TItem[] items)
        {
            return AddItem(opName, behavior, items);
        }

        public int Add(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks)
        {
            return AddBlock(op, behavior, blocks);
        }

        public int Add(string opName, InsertionBehavior behavior, params TBlock[] blocks)
        {
            return AddBlock(opName, behavior, blocks);
        }

        #endregion Add

        #region AddItem

        /// <summary>
        /// 增加查询项.
        /// </summary>
        /// <param name="item">要添加的项对象</param>
        /// <param name="behavior">是否可重复增加项(包括操作符)</param>
        /// <param name="op">逻辑连接操作符,与父组内其它条件之间的逻辑关系</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool InternalAddItem(TItem item, TOpEnum op, InsertionBehavior behavior)
        {
            if (item == null) return false;

            var itemDataType = GetItemOrgDataType(item);
            if (itemDataType != null)
            {
                if (_matchedItemsType != null) //本块内所含各项与项，项与块整集之间的匹配类型
                {
                    var tmpType = TypeMatcher.MatchDataTtype(_matchedItemsType, itemDataType, out _);
                    if (tmpType != null)
                        _matchedItemsType = tmpType;
                    else //匹配失败
                        throw new ArithmeticException(MsgStrings.InvalidTypeConvert(itemDataType,_matchedItemsType));
                }
                else
                {
                    _matchedItemsType = itemDataType; //首次加入的具有非空值项，               
                }
            }
            ReMatchDataType();
            _itemVersion++;
            _hashCodeVersion++;
            return m_items.Add(op, item, behavior);
        }

        public int AddItem(TOpEnum op, InsertionBehavior behavior, params TItem[] itemArray)
        {
            var id = 0;
            foreach (TItem p in itemArray)
            {
                if (InternalAddItem(p, op, behavior))
                    id++;
            }
            return id;
        }

        public int AddItem(string opName, InsertionBehavior behavior, params TItem[] itemArray)
        {
            if (GetOperatorWithString == null)
                throw new Exception(string.Format(MsgStrings.NullGetOperatorWithStringDelegate, nameof(GetOperatorWithString)));

            var op = GetOperatorWithString(opName);
            return AddItem(op, behavior, itemArray);
        }

        #endregion AddItem

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void Reduce()
        //{
        //    if(ItemCount==0 && BlockCount==1)
        //    {
        //        _blockVersion++;
        //        _itemVersion++;
        //        _hashCodeVersion++;
        //     
        //            var bk = m_blocks[0].Value;
        //            m_blocks.Clear();
        //            foreach(var im in bk.GetItems())
        //                AddItem(im.Key, false, im.Value);
        //            foreach (var ib in bk.GetBlocks()) 
        //            {
        //               // ib.Value.Reduce();
        //                AddBlock(ib.Key, false, ib.Value);
        //            }
        //     
        //    }

        //}

        #region AddBlock

        /// <summary>
        /// 增加块
        /// </summary>
        /// <param name="block">要添加的块</param>
        /// <param name="op">操作接符,与父组内其它块或项之间的操作符,</param>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool InternalAddBlock(TBlock block, TOpEnum op, InsertionBehavior behavior)
        {
            if (block == null)
                return false;

            //var blockType = typeof(TBlock);
            //var blockAllCount = blockType.GetProperty(nameof(AllCount))?.GetValue(block) ?? 0;

            var blockAllCount = block.AllCount;
            if ((int)blockAllCount == 0)
                return false;


            //var blockParent = blockType.GetProperty(nameof(ParentBlock));
            //blockParent.SetValue(block, this);
            var subBlockDataType = GetBlockOrgDataType(block);
            if (subBlockDataType != null)
            {
                if (_matchedSubBlocksDataType != null) //本块内所含各子块之间的匹配类型
                {
                    var tmpType = TypeMatcher.MatchDataTtype(_matchedSubBlocksDataType, subBlockDataType, out _);
                    if (tmpType != null)
                        _matchedSubBlocksDataType = tmpType;
                    else //匹配失败
                        throw new ArithmeticException(MsgStrings.InvalidTypeConvert( subBlockDataType,_matchedSubBlocksDataType));
                }
                else
                {
                    _matchedSubBlocksDataType = subBlockDataType; //加入的是第一个具有块类型的子块，               
                }

                //if (_matchedDataType != null) //需要重新匹配本块的类型
                //{
                //    tmpType = TypeMatcher.MatchDataTtype(_matchedDataType, _matchedSubBlocksDataType, out _);
                //    if (tmpType != null)
                //        _matchedDataType = tmpType;
                //    else  //匹配失败
                //        throw new ArithmeticException(MsgStrings.InvalidTypeConvert( subBlockDataType));
                //}
                //else
                //{
                //    _matchedDataType = _matchedSubBlocksDataType;//首次确认本块的整体类型
                //}

                ReMatchDataType();
            }


            _blockVersion++;
            _hashCodeVersion++;
            return m_blocks.Add(op, block, behavior);

        }

        public int AddBlock(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks)
        {
            var id = 0;
            foreach (TBlock p in blocks)
            {

                if (InternalAddBlock(p, op, behavior))
                    id++;
            }
            return id;
        }

        public int AddBlock(string opName, InsertionBehavior behavior, params TBlock[] blocks)
        {
            if (GetOperatorWithString == null)
                throw new Exception(string.Format(MsgStrings.NullGetOperatorWithStringDelegate, nameof(GetOperatorWithString)));

            var op = GetOperatorWithString.Invoke(opName);
            return AddBlock(op, behavior, blocks);
        }

        #endregion AddParamBlock

        #region Remove,Clear

        /// <summary>
        /// 移除指定的键位的项
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveItemAt(int index)
        {
            var mType = GetItemOrgDataType(m_items[index]?.Node); ;
            _itemVersion++;
            _hashCodeVersion++;
            if (m_items.RemoveAt(index))
            {
                ReMatchForItemsChanged(mType);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除所有指定的独立条件
        /// </summary>
        /// <param name="param"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveItem(TItem item)
        {
            try
            {
                var ids = 0;
                var mType = GetItemOrgDataType(item);
                for (var i = 0; i < m_items.Count; i++)
                {
                    var m = m_items[i];
                    if (m.Node == item)
                    {
                        var rdc = m_items.Remove(m.Operator, m.Node, false);
                        i--;
                        ids += rdc;
                    }
                }
                if (ids > 0)
                {
                    ReMatchForItemsChanged(mType);
                }
                return ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveItem(TItem item, TOpEnum op, bool byReference)
        {
            try
            {
                var mType = GetItemOrgDataType(item);
                var rt = m_items.Remove(op, item, byReference);
                if (rt > 0)
                {
                    ReMatchForItemsChanged(mType);
                }
                return rt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 移除指定键的条件组
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveBlockAt(int index)
        {
            try
            {
                _blockVersion++;
                _hashCodeVersion++;
                var bkType = GetBlockOrgDataType(m_blocks[index]?.Node);
                if (m_blocks.RemoveAt(index))
                {
                    ReMatchForSubBlocksChanged(bkType);
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        /// <summary>
        /// 移除指定的条件组
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveBlock(TBlock block)
        {
            try
            {
                var bkType = GetBlockOrgDataType(block);
                var ids = 0;
                for (var i = 0; i < m_blocks.Count; i++)
                {
                    var m = m_blocks[i];
                    if (m.Node == block)
                    {
                        var rdc = m_blocks.Remove(m.Operator, m.Node, false);
                        i--;
                        ids += rdc;
                    }
                }
                if (ids > 0)
                {
                    ReMatchForSubBlocksChanged(bkType);
                }
                return ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveBlock(TBlock block, TOpEnum op, bool byReference)
        {
            try
            {
                var bkType = GetBlockOrgDataType(block);
                var rt = m_blocks.Remove(op, block, byReference);
                if (rt > 0)
                {
                    ReMatchForSubBlocksChanged(bkType);
                }
                return rt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 单独清除内含条件组
        /// </summary>
        public void ClearBlocks()
        {
            _blockVersion++;
            _hashCodeVersion++;
            m_blocks.Clear();
            _blockVersion = 0;
            _matchedSubBlocksDataType = null;
            _matchedDataType = _matchedItemsType;

        }

        /// <summary>
        /// 清除独立条件
        /// </summary>
        public void ClearItems()
        {
            _itemVersion++;
            _hashCodeVersion++;
            m_items.Clear();
            _itemVersion = 0;
            _matchedItemsType = null;
            _matchedDataType = _matchedSubBlocksDataType;
        }

        /// <summary>
        /// 清除所有条件
        /// </summary>
        public void ClearAll()
        {
            ClearItems();
            ClearBlocks();
            _hashCodeVersion = 0;
        }

        #endregion Remove ,Clear

        #region Get:Item,Block,Operater


        /// <summary>
        /// 返回块内的项与操作的配对
        /// </summary>
        /// <param name="item">index 索引位置(从0计起)</param>
        public NodeOperatorPair<TOpEnum, TItem>[] GetItems()
        {
            return m_items.ToArray();
        }

        public NodeOperatorPair<TOpEnum, TItem> GetItemPair(int index)
        {
            return m_items[index];
        }

        public NodeOperatorPair<TOpEnum, TBlock>[] GetBlocks()
        {
            return m_blocks.ToArray();
        }

        /// <summary>
        /// 返回一个子块对象
        /// </summary>
        /// <param name="index">index 索引位置(从0计起)</param>
        public NodeOperatorPair<TOpEnum, TBlock> GetBlockPair(int index)
        {
            return m_blocks[index];
        }


        #endregion Get:Item .....

        #region Contains



        /// <summary>
        /// 检测块是否包含指定项,连同对应的操作符,使用项的相等比较(==).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(TItem item, TOpEnum op)
        {
            return m_items.Contains(op, item, false);

        }


        /// <summary>
        /// 检测块是否包含指定块,连同对应的操作符,使用块的相等比较(==).
        /// </summary>
        /// <param name="block"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(TBlock block, TOpEnum op)
        {
            return m_blocks.Contains(op, block, false);

        }

        #endregion //contains

        #region Propertys


        /// <summary>
        /// 默认为根据枚举名称获取枚举值,可以另指定此委托.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private protected Func<string, TOpEnum> GetOperatorWithString { get; set; } = (n) =>
        {
            var r = Enum.TryParse(typeof(TOpEnum), n, true, out var op) ? op : default(QueryArithmetic);
            return (TOpEnum)r;
        };


        /// <summary>
        /// 块内项的计数
        /// </summary>
        public int ItemCount => m_items.Count;

        /// <summary>
        /// 块内子块的计数
        /// </summary>
        public int BlockCount => m_blocks.Count;

        /// <summary>
        /// 返回块内的项和子块的计数(非递归计数)
        /// </summary>
        public int AllCount => m_items.Count + m_blocks.Count;


        /// <summary>
        /// 是否自动简化,如果是,则当空块或只包含一个子块且没有独立项的块在被添加时,会被抛弃,而用其内含子块取代之.
        /// </summary>
        public bool AutoReduce { get; set; } = true;

        //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public NodeOperatorPair<TOpEnum, TItem>[] Items
        {
            get
            {
                return GetItems();
            }
            set
            {
                ReMatchItemsDataType(value);
                _itemVersion = 0;
                _hashCodeVersion = 0;
                m_items.Items = value;
            }
        }

        //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public NodeOperatorPair<TOpEnum, TBlock>[] Blocks
        {
            get
            {
                return GetBlocks();
            }
            set
            {
                ReMatchSubBlocksDataType(value);
                _blockVersion = 0;
                _hashCodeVersion = 0;
                m_blocks.Items = value;
            }
        }

        /// <summary>
        /// 已匹配的块集数据类型,它表示块内的项与项之间，项与子块集合整体之间的匹配类型
        /// </summary>
        public override Type DataType => _matchedDataType;
        /// <summary>
        /// 已匹配的项集数据类型
        /// </summary>
       // public Type MatchedItemsDataType => _matchedItemsDataType;

        public Type MatchedSubBlocksDataType => _matchedSubBlocksDataType;
        #endregion Propertys   

        #region private methods

        private void ClassInitialize()
        {
            m_items = new NodeCollection<TOpEnum, TItem>();
            m_blocks = new NodeCollection<TOpEnum, TBlock>();
            _equalComparer = new QueryEqualComparer<TBlock>((x, y) =>
            {
                if (y is TBlock obj)
                {
                    if (ReferenceEquals(this, obj)) return true;
                    if (ReferenceEquals(obj, null)) return false;
                    if (ItemCount != obj.ItemCount || BlockCount != obj.BlockCount) return false;
                    return this.GetHashCode() == obj.GetHashCode();
                }
                else return false;

            });

        }

        /// <summary>
        /// 用于批量填充Items重新匹配类型
        /// </summary>
        /// <param name="items"></param>
        private void ReMatchItemsDataType(NodeOperatorPair<TOpEnum, TItem>[] items)
        {
            if (items != null && items.Length > 0)
            {
                int start;
                var count = items.Length;
                var item = items[0]?.Node;
                for (start = 1; start < count; start++)  //拿到第一个有效项
                {
                    item = items[start]?.Node;
                    if (item != null) break;
                }

                Type tmpType, lastType;
                lastType = GetItemOrgDataType(item);
                for (int i = start - 1; i < count; i++)
                {
                    item = items[i]?.Node;
                    tmpType = GetItemOrgDataType(item);
                    if (tmpType != null)
                    {
                        if (lastType != null)
                        {
                            tmpType = TypeMatcher.MatchDataTtype(lastType, tmpType, out _);
                            if (tmpType != null)
                                lastType = tmpType;
                            else//匹配失败
                                throw new ArithmeticException(MsgStrings.InvalidTypeConvert(lastType,GetItemOrgDataType(item)));
                        }
                        else
                        {
                            lastType = tmpType;
                        }
                    }//else 空值没有类型匹配
                }
                _matchedItemsType = lastType;
            }
            else
            {
                _matchedItemsType = null;
            }
            ReMatchDataType();

        }

        /// <summary>
        /// 用于批量填充Blocks重新匹配类型
        /// </summary>
        /// <param name="blocks"></param>
        private void ReMatchSubBlocksDataType(params NodeOperatorPair<TOpEnum, TBlock>[] blocks)
        {
            if (blocks != null && blocks.Length > 0)
            {
                var count = blocks.Length;
                var block = blocks[0]?.Node;
                Type tmpType, lastType = GetBlockOrgDataType(block);
                for (int i = 1; i < count; i++)
                {
                    block = blocks[i]?.Node;
                    tmpType = block?.TypeAs?.GetElementType() ?? block?.DataType?.GetElementType();
                    if (tmpType != null)
                    {
                        if (lastType != null)
                        {
                            tmpType = TypeMatcher.MatchDataTtype(lastType, tmpType, out _);
                            if (tmpType != null)
                                lastType = tmpType;
                            else//匹配失败
                                throw new ArithmeticException(MsgStrings.InvalidTypeConvert( lastType,GetBlockOrgDataType(block)));
                        }
                        else
                        {
                            lastType = tmpType;
                        }
                    }//else 空值没有类型匹配
                }
                _matchedSubBlocksDataType = lastType;
            }
            else
            {
                _matchedSubBlocksDataType = null;
            }
            ReMatchDataType();
        }

        /// <summary>
        /// 项更改后，重新进行匹配
        /// </summary>
        /// <param name="changedItemDataType"></param>
        private void ReMatchForItemsChanged(Type changedItemDataType)
        {
            if (m_items.Count == 0)
            {
                _matchedItemsType = null;
                _matchedDataType = _matchedSubBlocksDataType;
                return;
            }
            if (changedItemDataType != null)
            {
                if (TypeMatcher.MatchDataTtype(_matchedItemsType, changedItemDataType, out _) != _matchedItemsType)
                {
                    ReMatchItemsDataType(GetItems());
                }
            }
        }

        /// <summary>
        /// 块更改后，进行重新匹配
        /// </summary>
        /// <param name="changedBlockDataType"></param>
        private void ReMatchForSubBlocksChanged(Type changedBlockDataType)
        {
            if (m_blocks.Count == 0)
            {
                _matchedSubBlocksDataType = null;
                _matchedDataType = _matchedItemsType;
                return;
            }
            if (changedBlockDataType != null)
            {
                if (TypeMatcher.MatchDataTtype(_matchedSubBlocksDataType, changedBlockDataType, out _) != _matchedSubBlocksDataType)
                    ReMatchSubBlocksDataType(GetBlocks());
            }
        }
        
        /// <summary>
        /// 在匹配项或块集后，进行项与块集整体之间的匹配
        /// </summary>
        private void ReMatchDataType()
        {
            if (_matchedItemsType != null && _matchedSubBlocksDataType != null)
            {
                _matchedDataType = TypeMatcher.MatchDataTtype(_matchedItemsType, _matchedSubBlocksDataType, out _);
                if (_matchedItemsType == null)
                    throw new ArithmeticException(MsgStrings.InvalidTypeConvert( _matchedDataType,_matchedSubBlocksDataType));
            }
            else if (_matchedItemsType != null)
                _matchedDataType = _matchedItemsType;
            else if (_matchedSubBlocksDataType != null)
                _matchedDataType = _matchedSubBlocksDataType;
            else
                _matchedDataType = null;
        }
               
        /// <summary>
        /// 获取项的待匹配输出数据类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Type GetItemOrgDataType(TItem item)
        {
            return item?.TypeAs?.GetCollectionElementType() ?? item?.DataType?.GetCollectionElementType();
        }

        /// <summary>
        /// 获取块的待匹配数据类型
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private Type GetBlockOrgDataType(TBlock block)
        {
            return block?.TypeAs?.GetCollectionElementType() ?? block.DataType;
        }


        #endregion private methods

        #region other public      

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        QueryEqualComparer<TBlock> _equalComparer;

        /// <summary>
        /// 获取相等比较器实例
        /// </summary>
        /// <returns></returns>
        public QueryEqualComparer<TBlock> GetEqualComparer() => _equalComparer;

        /// <summary>
        /// 设置相等比较器实例
        /// </summary>        
        public void SetEqualComparer(QueryEqualComparer<TBlock> value)
        {

            if (value == null) throw new ArgumentNullException("SetEqualComparer", MsgStrings.ValueCannotNull("QueryBlock"));
            _equalComparer = value as QueryEqualComparer<TBlock>
                ?? throw new InvalidCastException(MsgStrings.InvalidTypeConvert( value?.GetType(),typeof(QueryEqualComparer<TBlock>)));

        }
       
        public override string ToString()
        {
            return "Block: ItemCount=" + ItemCount.ToString() + "BlockCount=" + BlockCount.ToString();
        }

        #endregion other public

        #region 实现接口相关

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int GetHashCode()
        {
            //var v = _hashCodeVersion;
            if (_hashCodeVersion == _lastHashCodeVersion) return _hashCodeCache;
            var redo = true;
            int hsCode = 0;
            long hsVer;
            while (redo)//防止其它线程中途的修改
            {
                hsVer = _hashCodeVersion;
                for (int i = 0; i < this.ItemCount; i++)
                {
                    if (hsVer == _hashCodeVersion)
                    {
                        var itm = m_items[i];
                        hsCode = CombineHashCode(itm.Node.GetHashCode(), itm.Operator.GetHashCode(), hsCode);
                    }
                    else
                        break;
                }
                for (int i = 0; i < this.BlockCount; i++)
                {
                    if (hsVer == _hashCodeVersion)
                    {
                        var bk = m_blocks[i];
                        hsCode = CombineHashCode(bk.Node.GetHashCode(), bk.Operator.GetHashCode(), hsCode);
                    }
                    else
                        break;
                }
                if (hsVer == _hashCodeVersion)
                {
                    _hashCodeCache = hsCode;
                    _lastHashCodeVersion = _hashCodeVersion;
                    redo = false;
                }
            }
            return hsCode;

        }

        public static bool operator ==(QueryBlock<TItem, TBlock, TOpEnum> x, object y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null)) return true;  //同时为null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false; //只有其中一个为null
            if (ReferenceEquals(x, y)) return true;  //是否同引用的相同实例
            if (!(y is QueryBlock<TItem, TBlock, TOpEnum> yt)) return false;
            return x.GetHashCode() == yt.GetHashCode();

        }

        public static bool operator !=(QueryBlock<TItem, TBlock, TOpEnum> x, object y)
        {
            return !(x == y);
        }

        public override bool Equals(object other)
        {
            if (!(other is QueryBlock<TItem, TBlock, TOpEnum> obj))
                return false;
            return Equals(obj);
        }

        public virtual bool Equals([AllowNull] QueryBlock<TItem, TBlock, TOpEnum> other)
        {
            return this == other;
        }

        //public override bool Equals([AllowNull] QueryNode other)
        //{
        //    return Equals((QueryBlock<TItem, TBlock, TOpEnum>)other);
        //}


        #endregion impliments interfaces

        //private void ClassTerminate()
        //{

        //}

        //~QueryParamBlock()
        //{
        //    ClassTerminate();
        //    //base.Finalize();
        //}
    }

    #endregion blockbase

}