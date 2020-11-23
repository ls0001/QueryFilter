using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{

    /// <summary>
    /// 查询表达式与操作符组合的集合
    /// </summary>
    /// <typeparam name="TOperator">操作符,可以是:逻辑,比较,计算</typeparam>
    /// <typeparam name="TValue">被操作表达式,可以是:列,值,条件组</typeparam>
    //[DebuggerTypeProxy(typeof(QueryPartsCollectionDebugView<,>))]
    [DebuggerDisplay("NodeCollection[{_count}] <{typeof(TOperator).Name,nq},{typeof(TValue).Name,nq}>")]
    [JsonConverter(typeof(NodeCollectionJsonConverter))]
    public partial class NodeCollection<TOperator, TValue> : IEnumerable<NodeOperatorPair<TOperator, TValue>>
       where TValue : QueryNode//, IEquatable<TValue>
       where TOperator : struct, Enum
    {

        #region 变量

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        InternalList<NodeOperatorPair<TOperator, TValue>> _members;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEqualityComparer<TValue> _comparer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        const int defaultBucketsSize = 4;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        const int freeLinkStart = -3;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _version = 0;

        /// <summary>
        /// member count(value count)
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _count = 0;

        /// <summary>
        /// link count (key count)
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _linkCount = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _freeLinkCount = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _freeLink = 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _capacity = defaultBucketsSize;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //int _linkFreeCount = defaultBucketsSize;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _bucketsLinkSize = defaultBucketsSize;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int[] _bucketsLink;

        /// <summary>
        /// keys
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Link[] _links;

        #endregion 变量


        #region 属性和索引
        public int Count { get => _count; }

        public NodeOperatorPair<TOperator, TValue>[] Items
        {
            get => _members.ToArray(0);
            set => ResetFromMembers(value);
        }

        /// <summary>
        /// 返回一个<typeparamref name="TOperator"/> 与 <typeparamref name="TValue"/> 的配对的对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public NodeOperatorPair<TOperator, TValue> this[int index]
        {
            get => _members[index];
        }

        #endregion 属性和索引


        #region 构造

        public NodeCollection(IEqualityComparer<TValue> comparer, int capacity = defaultBucketsSize)
        {
            _comparer = comparer;
            _capacity = capacity;
            initClass(capacity);
        }
        public NodeCollection() : this(new QueryEqualComparer<TValue>(), defaultBucketsSize) { }


        #endregion


        #region private methods

        void initClass(int capacity)
        {
            _count = 0;
            _linkCount = 0;
            _version = 0;
            _freeLink = 0;
            _freeLinkCount = 0;
            _bucketsLinkSize = capacity;
            _bucketsLink = new int[_bucketsLinkSize];
            _links = new Link[_bucketsLinkSize];
            _members = new InternalList<NodeOperatorPair<TOperator, TValue>>(_bucketsLinkSize);
        }

        /// <summary>
        /// 如果比较器为空,采用object.GetHashCode(非TValue实例本身默认比较器的GetHashCode)
        /// </summary>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int GetMemberHashCode(TOperator op, TValue value)
        {
            Debug.Assert(value != null, "不能为空!");
            var comparer = _comparer;
            //var valueHsCode = comparer == null ? ((object)value)?.GetHashCode()?? 0: comparer.GetHashCode(value);
            var valueHsCode = comparer?.GetHashCode(value) ?? value?.GetHashCode() ?? 0;
            //return HashCode.Combine(op?.GetHashCode()??0, valueHashCode);
            return HashCode.Combine(op, valueHsCode);
        }

        double GetResizefactor(int count)
        {
            var a = defaultBucketsSize < 2 ? 2 : defaultBucketsSize;
            var b = count < 2 ? 2 : count;
            if (b < a) return 2;
            var p1 = Math.Pow(Math.Log(a, b), 2);
            return 1 + 10 * p1;

        }

        void ResizeLinks(int newSize)
        {
            if (_members == null)
                return;
            if (newSize <= _linkCount)
                newSize = (int)(_linkCount * GetResizefactor(_linkCount));

            var newLinkbuckets = new int[newSize];
            var newLinks = new Link[newSize];
            var links = _links;

            Array.Copy(links, 0, newLinks, 0, _linkCount);

            //重新分桶
            for (int i = 0; i < _linkCount; i++)
            {
                if (newLinks[i].next >= -1)
                {
                    uint bucketindex = newLinks[i].hashCode % (uint)newSize;
                    // 从1开始计数
                    newLinks[i].next = newLinkbuckets[bucketindex] - 1;
                    newLinkbuckets[bucketindex] = i + 1;
                }
            }

            _bucketsLink = newLinkbuckets;
            _links = newLinks;
            // * _linkFreeCount = newSize - _linkCount;
        }

        bool TryInsert(TOperator op, TValue value, InsertionBehavior behavior = InsertionBehavior.Duplicates)
        {
            if (value == null)
                return false;
            var members = _members;
            Debug.Assert(members != null, MsgStrings.ExpectedMemberListNull);
            var comparer = _comparer;
            var memberHashCode = (uint)GetMemberHashCode(op, value);
            var linkBucketIndex = (int)(memberHashCode % (uint)_bucketsLink.Length);
            ref var linkBucket = ref _bucketsLink[linkBucketIndex];
            var i = linkBucket - 1;   //bucket记录的是从1算起(链的索引+1),所以应该减回1
            var loopLinkCount = 0;
            //找到的目标元素所在的链
            var existsLinkIndex = -1;

            do //查询链,
            {
                if ((uint)i >= (uint)_links.Length)
                    break;

                if (_links[i].hashCode == memberHashCode) //找到键//检测重复//向前查找(桶的存储始终是最未端的链索引+1)
                {
                    var j = _links[i].members[0];
                    if (members[j].hashCode == memberHashCode)
                    {
                        if (behavior == InsertionBehavior.Overwrite || members[j].Node == null)
                        {
                            members[j].Node = value;
                            _version++;
                            return true;
                        }
                        if (behavior == InsertionBehavior.Duplicates)
                        {
                            existsLinkIndex = i;
                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                //如果循环过程中,发生了并发插入
                if (loopLinkCount >= _links.Length)
                    throw new NotSupportedException();

                i = _links[i].next; //向上链
                loopLinkCount++;
            } while (true);

            _version++;
            //新建一个元素对象,把值放进其中
            NodeOperatorPair<TOperator, TValue> member = new NodeOperatorPair<TOperator, TValue>(op, value);
            member.hashCode = memberHashCode;

            //不存在相同元素,增加一个链
            if (existsLinkIndex == -1)
            {
                int linkIndex;
                //如果有自由链(删除完链的元素后,链空闲了,进行复用)
                if (_freeLinkCount > 0)
                {
                    linkIndex = _freeLink;
                    _freeLinkCount--;
                    var nextfreeLink = freeLinkStart - _links[_freeLink].next;
                    Debug.Assert(nextfreeLink >= -1, "溢出错误");
                    _freeLink = nextfreeLink;
                }
                else
                {
                    if (_linkCount >= _bucketsLink.Length)
                    {
                        ResizeLinks(_linkCount);
                        linkBucketIndex = (int)(memberHashCode % (uint)_bucketsLink.Length);
                        linkBucket = ref _bucketsLink[linkBucketIndex];
                    }
                    linkIndex = _linkCount;
                    _linkCount++;
                }
                //*_linkFreeCount--;
                //对链进行分桶
                //*var linkIndex = _linkCount;
                ref var link = ref _links![linkIndex];
                link.members = new InternalList<int>();   //指向刚新建的元素
                link.members.Add(_count);
                link.hashCode = memberHashCode;
                link.next = linkBucket - 1;                 //相邻链的桶值(邻链的索引)                                                            
                linkBucket = linkIndex + 1;                 //link桶所记录的值,总是比link的真实索引大1(链在桶中从1开始计数)
                //*_linkCount++;
            }
            else
            //已有相同元素
            {
                //把重复的元素的索引加入链的元素列表中
                _links[existsLinkIndex].members.Add(_count);
            }
            _count++;
            members.Add(member);  //处理完所有属性后将元素加入列表
            return true;
        }

        int[] FindMembers(TOperator op, TValue value, bool byReference = false)
        {
            int[] result = new int[0];
            if (value == null)
                return result;

            var buckets = _bucketsLink;
            var members = _members;
            var loopCount = 0;
            var findedCount = 0;

            if (buckets == null)
                return result;

            //Debug.Assert(members != null, MsgStrings.ExpectedEntriesNull);
            var comparer = _comparer;
            uint hashCode = (uint)GetMemberHashCode(op, value);
            // 链在桶中是从1开始计数的,这里应减回1与从0计数的元素集匹配
            var i = buckets[hashCode % (uint)buckets.Length] - 1;
            var findedMember = new InternalList<int>();
            do
            {
                if ((uint)i >= (uint)_links.Length)
                    return result;

                if (_links[i].hashCode == hashCode)
                {
                    var iCount = _links[i].members.Count;
                    //遍历链所包含的元素
                    for (var k = 0; k < _links[i].members.Count; k++) //k of 链的所包含的元素索引 
                    {
                        //取出元素
                        var e = _links[i].members[k];
                        var member = members[e];
                        //比较其值
                        var valueEqual = byReference ?
                                object.ReferenceEquals(member.Node, value) :
                                _comparer?.Equals(member.Node, value) ?? EqualityComparer<TValue>.Default.Equals(member.Node, value);
                        //值和操作符都相等
                        if (member.Operator.Equals(op) && valueEqual)
                        {
                            //循环过程中插入了本键重复的项//此种情况本应禁止,这里允许
                            if (findedCount >= iCount)
                            { }
                            findedMember.Add(e);
                            //result[findedCount] = e;
                            if (byReference)
                                break;

                            findedCount++;
                        }
                    }
                    break;
                }

                i = _links[i].next;
                if (loopCount >= _links.Length)
                {
                    //如果循环过程中,插入了项,或发生了死链
                    throw new NotSupportedException();
                }
                loopCount++;
            } while (true);

            result = findedMember.ToArray(0, findedMember.Count);
            return result;
        }

        /// <summary>
        /// 通过设置成员列表,恢复集合
        /// </summary>
        void ResetFromMembers(NodeOperatorPair<TOperator, TValue>[] memberArray)
        {
            if (memberArray == null || memberArray.Length == 0)
            {
                Clear();
                return;
            }

            _count = 0;
            _linkCount = 0;
            _version = 0;
            _freeLink = 0;
            _freeLinkCount = 0;
            _bucketsLinkSize = memberArray.Length;
            _bucketsLink = new int[_bucketsLinkSize];
            _links = new Link[_bucketsLinkSize];
            _members = new InternalList<NodeOperatorPair<TOperator, TValue>>();
            var members = memberArray;
            foreach (var mb in members)
            {
                if (mb == null) continue;
                var op = mb.Operator;
                var value = mb.Node;
                // if (op == null || mb.Node == null) continue;
                var comparer = _comparer;
                var memberHashCode = (uint)GetMemberHashCode(op, value);
                var linkBucketIndex = (int)(memberHashCode % (uint)_bucketsLink.Length);

                ref var linkBucket = ref _bucketsLink[linkBucketIndex];
                var i = linkBucket - 1;   //bucket记录的是从1算起(链的索引+1),所以应该减回1
                var loopLinkCount = 0;
                //找到的目标元素所在的链
                var existsLinkIndex = -1;

                do //查询链,
                {
                    if ((uint)i >= (uint)_links.Length)
                        break;

                    if (_links[i].hashCode == memberHashCode) //找到键//检测重复//向前查找(桶的存储始终是最未端的链索引+1)
                    {
                        var j = _links[i].members[0];
                        if (members[j].hashCode == memberHashCode)
                        {
                            existsLinkIndex = i;
                            break;
                        }
                    }
                    //如果循环过程中,发生了并发插入
                    if (loopLinkCount >= _links.Length)
                        throw new NotSupportedException();

                    i = _links[i].next; //向上链
                    loopLinkCount++;
                } while (true);

                _version++;
                mb.hashCode = memberHashCode;

                //不存在相同元素,增加一个链
                if (existsLinkIndex == -1)
                {
                    int linkIndex;
                    linkIndex = _linkCount;
                    _linkCount++;
                    //对链进行分桶
                    ref var link = ref _links![linkIndex];
                    link.members = new InternalList<int>();   //指向刚新建的元素
                    link.members.Add(_count);
                    link.hashCode = memberHashCode;
                    link.next = linkBucket - 1;                 //相邻链的桶值(邻链的索引)                                                            
                    linkBucket = linkIndex + 1;                 //link桶所记录的值,总是比link的真实索引大1(链在桶中从1开始计数)
                                                                //_linkCount++;
                }
                else
                //已有相同元素
                {
                    //把重复的元素的索引加入链的元素列表中
                    _links[existsLinkIndex].members.Add(_count);
                }
                _members.Add(mb);
                _count++;
            }

        }

        #endregion //private methods


        #region public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">操作符:逻辑,比较,计算,</param>
        /// <param name="value"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public bool Add(TOperator op, TValue value, InsertionBehavior behavior = InsertionBehavior.Overwrite)
        {
            return TryInsert(op, value, behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <param name="byReference">是否采用对象相同引用的比较(而不是采用比较器的相等比较)进行查找匹配</param>
        /// <returns></returns>
        public int Remove(TOperator op, TValue value, bool byReference = true)
        {
            int removedCount = 0;
            if (value == null)
                return removedCount;

            int[] buckets = _bucketsLink;
            var members = _members;
            int loopsCount = 0;
            //if (buckets != null)
            //{
            //Debug.Assert(members != null, MsgStrings.ExpectedEntriesNull);
            uint hashCode = (uint)GetMemberHashCode(op, value);
            uint bucket = hashCode % (uint)buckets.Length;
            int last = -1;
            // 从1开始计数
            int i = buckets[bucket] - 1;
            while (i >= 0)
            {
                ref Link link = ref _links[i];
                //找到对应的链
                if (link.hashCode == hashCode)
                {
                    for (var k = link.members.Count - 1; k >= 0; --k) //遍历包含的每个元素
                    {
                        var e = link.members[k];
                        var member = members[e];
                        var valueEqual = byReference ?
                            object.ReferenceEquals(member.Node, value) :
                            _comparer?.Equals(member.Node, value) ?? EqualityComparer<TValue>.Default.Equals(member.Node, value);
                        //如果找到要删除元素
                        if (member.Operator.Equals(op) && valueEqual)
                        {
                            //如果只包含一个元素,把链从桶中删除
                            if (link.members.Count == 1)
                            {
                                if (last < 0)
                                    buckets[bucket] = link.next + 1;            //从1起数,如果第一次直接命中,桶指向下一个链
                                else
                                    _links[last].next = link.next;             //用上一个被检查的链替换被命中链

                                link.next = freeLinkStart - _freeLink;          //置为空闲链
                                link.members = null;

                                //标记空闲链索引
                                _freeLink = i;
                                _freeLinkCount++;
                            }
                            else
                            {
                                link.members.RemoveAt(k);
                            }
                            members.RemoveAt(e);
                            removedCount++;
                            _count--;
                            //所有指向k后面元素的link都要跟着变,// 第k个后面的将跟上来(索引都会变小1)
                            for (var ik = 0; ik < _linkCount; ik++)
                            {
                                var link_k = _links[ik];
                                if (link_k.members == null)
                                    continue;
                                for (var ie = link_k.members.Count - 1; ie >= 0; --ie)
                                {
                                    var link_member_ie = link_k.members[ie];
                                    if (link_member_ie < e) break; //前面的只会更小
                                    if (link_member_ie > e)
                                        link_k.members[ie] = --link_member_ie;
                                }
                            }

                            if (byReference || link.members == null)
                                return removedCount;

                        }

                    }
                    return removedCount;

                }

                last = i;
                i = link.next;

                if (loopsCount >= _links.Length)
                {
                    throw new NotSupportedException();
                }
                loopsCount++;

            }
            return removedCount;
        }

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                return false;
            var member = _members[index];
            return Remove(member.Operator, member.Node, true) > 0;
        }

        public void Clear()
        {

            //            _linkFreeCount = 0;
            initClass(_capacity);

        }

        /// <summary>
        /// 返回一个数组,它包含了集合中所有与指定值<paramref name="value"/>相等(或相同)的值<paramref name="value"/>的索引
        /// </summary>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <param name="byReference">是否采用对象相同引用的比较(而不是采用比较器的相等比较)进行查找匹配</param>
        /// <returns></returns>
        public int[] IndexOf(TOperator op, TValue value, bool byReference = false)
        {
            return FindMembers(op, value, byReference);
        }

        /// <summary>
        /// 是否已包含
        /// </summary>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <param name="byReference">是否采用对象相同引用的比较(而不是采用比较器的相等比较)进行查找匹配</param>
        /// <returns></returns>
        public bool Contains(TOperator op, TValue value, bool byReference = false)
        {
            return FindMembers(op, value, byReference).Length == 0;
        }

        public NodeOperatorPair<TOperator, TValue>[] ToArray()
        {
            return _members.ToArray(0, _count);
        }

        public override string ToString()
        {
            return $"QueryNodeCollections[{_count}]<{typeof(TOperator).Name},{typeof(TValue).Name}>";
        }

        #endregion public methods


        #region 实现枚举接口

        public IEnumerator<NodeOperatorPair<TOperator, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        internal struct Enumerator : IEnumerator<NodeOperatorPair<TOperator, TValue>>, IEnumerator
        {
            private readonly NodeCollection<TOperator, TValue> _members;
            private int _index;
            private readonly int _version;
            private NodeOperatorPair<TOperator, TValue> _current;

            internal Enumerator(NodeCollection<TOperator, TValue> members)
            {
                _members = members;
                _index = 0;
                _version = members._version;
                _current = default;
            }
            public bool MoveNext()
            {
                if (_version == _members._version && ((uint)_index < (uint)_members._count))
                {
                    _current = _members._members[_index];
                    _index++;
                    return true;
                }

                if (_version != _members._version)
                {
                    throw new InvalidOperationException();
                }

                _index = _members._count + 1;
                _current = default;
                return false;

            }

            void IEnumerator.Reset()
            {
                if (_version != _members._version)
                {
                    throw new InvalidOperationException();
                }

                _index = 0;
                _current = default;
            }

            public NodeOperatorPair<TOperator, TValue> Current => _current!;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _members._count + 1)
                    {
                        throw new InvalidOperationException();
                    }
                    return Current;
                }
            }

            public void Dispose()
            {
            }

        }

        #endregion //实现枚举接口

    }





}
