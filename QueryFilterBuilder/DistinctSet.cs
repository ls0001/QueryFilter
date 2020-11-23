using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 简单有序哈希表.
    /// </summary>
    /// <typeparam name="T">项的类型</typeparam>
    [DebuggerDisplay("DistinctSet[{Count}] <{typeof(T).Name,nq}>")]
    public sealed class DistinctSet<T>
    {
        #region variables

        private const int _defaultCapacity = 1;

        /// <summary>
        /// 记录初始化时传入的初始大小
        /// </summary>
        private int _capacity = _defaultCapacity;

        /// <summary>
        /// 用于哈希和比较集合中的项的比较器
        /// </summary>
        private readonly IEqualityComparer<T> _comparer;

        /// <summary>
        /// 哈希桶，存储插槽的索引+1的值。
        /// </summary>
        private int[] _buckets;

        private Slot[] _slots;

        private int _allIndexCount;

        /// <summary>
        /// 已删除项的计数
        /// </summary>
        private int _freeIndexCount = 0;

        private long _version;
        private long _lastVersion;
        T[] _toArray;
        List<T> _toList;

        #endregion //variables

        #region ctor.

        /// <summary>
        ///  
        /// </summary>
        /// <param name="comparer">
        /// 用于哈希和比较集合中的项的比较器
        /// 如果为<c>null</c>,则默认使用 <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        public DistinctSet([AllowNull] IEqualityComparer<T> comparer) : this()
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;

        }

        public DistinctSet(int capacity = _defaultCapacity)
        {
            _capacity = capacity;
            if (_capacity < 1)
            {
                _capacity = 1;
            }
            InitClass();
            _comparer = EqualityComparer<T>.Default;
        }

        #endregion //ctor.

        #region property

        /// <summary>
        /// 本集合中的项目计数.
        /// </summary>
        public int Count => _allIndexCount - _freeIndexCount;

        #endregion //property

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>此方法内部执行索引扫描而速度甚忧，不建议频繁使用，如果要进行枚举请使用
        /// <see langword="Enumerate(action(T))"/>方法，
        /// 如果频繁执行请使用<see langword="ToArray&lt;T&gt;"/>，<see langword="ToList&lt;T&gt;"/>的结果。</remarks>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _allIndexCount - _freeIndexCount)
                    throw new IndexOutOfRangeException();

                int i;
                int id = -1;
                for (i = 0; i < _allIndexCount; i++)   //出于节省内存考虑，而没有存储索引映射表，需要执行索引扫描。
                {
                    if (_slots[i].Next == -2)
                        continue;
                    if (++id != index)
                        continue;

                    return _slots[i].Value;
                }
                throw new InvalidOperationException();
            }
        }


        /// <summary>
        /// 尝试将项目添加到本集合
        /// </summary>
        /// <param name="value">要添加的项.</param>
        /// <returns>
        /// <see langword="True"/> 如果项目不在集合中；否则 <see langword="False"/>.
        /// </returns>
        public bool Add(T value)
        {
            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % _buckets.Length;
            for (int i = _buckets[bucket] - 1; i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].Next == -2)
                    break;
                if (_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, value))
                {
                    return false;
                }
            }
            if (_allIndexCount == _slots.Length)
            {
                Resize();
            }
            int index = _allIndexCount;
            bucket = hashCode % _buckets.Length;
            _slots[index].HashCode = hashCode;
            _slots[index].Value = value;
            _slots[index].Next = _buckets[bucket] - 1;  //.next是前排邻居的真实索引
            _buckets[bucket] = index + 1;               //对应桶值=索引+1
            _allIndexCount++;
            _version++;
            return true;
        }

        /// <summary>
        /// 删除一个项.
        /// </summary>
        /// <param name="value">要被删除的项</param>
        /// <returns>
        /// <see langword="True"/>  删除删除成功; 否则 <see langword="False"/> .
        /// </returns>
        public bool Remove(T value)
        {
            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % _buckets.Length;
            int last = -1;
            for (int i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].Next)
            {
                if (_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, value))
                {
                    if (last < 0)                                //上次检测项没有前排邻居，或者当前是首次进入直接被命中
                        _buckets[bucket] = _slots[i].Next + 1;   //把对应桶交给邻居
                    else
                    {
                        _slots[last].Next = _slots[i].Next;      //当前项从桶链中脱离
                    }
                    _slots[i].HashCode = -1;
                    _slots[i].Next = -2;
                    _slots[i].Value = default;
                    _freeIndexCount++;
                    _version++;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除指定位置的项。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>此方法内部执行索引扫描而速度不佳，建议优先使用<see langword="Remove(T)"/></remarks>
        public T RemoveAt(int index)
        {
            if (index < 0 || index >= _allIndexCount - _freeIndexCount)
                throw new IndexOutOfRangeException();

            int id = -1;
            for (int i = 0; i < _allIndexCount; i++)
            {
                if (_slots[i].Next == -2)
                    continue;
                if (++id != index)
                    continue;

                var solt = _slots[i];
                int bucket = solt.HashCode % _buckets.Length;
                int last = _buckets[bucket] - 1;
                if (!(i == last))  //如果不是最后排
                {
                    for (int j = last; j >= 0; j = _slots[j].Next)
                    {
                        if (i == _slots[j].Next) //后排邻居
                        {
                            //当前项从桶链中脱离
                            _slots[j].Next = _slots[i].Next;
                            break;
                        }
                    }
                }
                else
                {
                    _buckets[bucket] = solt.Next + 1;
                }
                var rval = _slots[i].Value;
                _slots[i].HashCode = -1;
                _slots[i].Next = -2;
                _slots[i].Value = default;
                _freeIndexCount++;
                _version++;
                return rval;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// 清空集合，重新初始化。
        /// </summary>
        public void Clear()
        {
            var count = Count;
            //是引用类型或包含引用的值类型
            if (count > 0 && RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_slots, 0, count);
            }
            InitClass();
        }

        /// <summary>
        /// 检测是否包含指定项
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see langword="true"/>，如果包含，否则:<see langword="false"/></returns>
        public bool Contains(T value)
        {
            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % _buckets.Length;
            for (int i = _buckets[bucket] - 1; i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].Next == -2)
                    return false;
                if (_slots[i].HashCode == hashCode && _comparer.Equals(_slots[i].Value, value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 等同Foreach
        /// </summary>
        /// <param name="action"></param>
        public void Enumerate(Action<T> action)
        {
            for (int i = 0; i < _allIndexCount; i++)
            {
                if (_slots[i].Next == -2)
                    continue;
                action(_slots[i].Value);
            }
        }

        /// <summary>
        /// 将项输出到数组Array&lt;<typeparamref name="T"/>&gt;。
        /// </summary>
        public T[] ToArray()
        {
            if (_lastVersion == _version)
                return _toArray;
            int index = 0;
            var arr = new T[_allIndexCount - _freeIndexCount];
            for (int i = 0; i < _allIndexCount; i++)
            {
                if (_slots[i].Next == -2)
                    continue;
                arr[index] = _slots[i].Value;
                index++;
            }
            _lastVersion = _version;
            _toArray = arr;
            return arr;
        }

        /// <summary>
        /// 将项输出到List&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        public List<T> ToList()
        {
            if (_lastVersion == _version)
                return _toList;
            var lst = new List<T>(_allIndexCount - _freeIndexCount);
            for (int i = 0; i != _allIndexCount; ++i)
            {
                if (_slots[i].Next == -2)
                    continue;
                lst.Add(_slots[i].Value);
            }
            _lastVersion = _version;
            _toList = lst;
            return lst;
        }

        /// <summary>
        /// 将本集合空间裁剪到实际大小，以减少内存占用。
        /// </summary>
        public void TrimExcess()
        {
            Resize(Count);
        }

        #endregion //public methods

        #region private methods

        private int InternalGetHashCode(T value) => value == null ? 0 : _comparer.GetHashCode(value) & 0x7FFFFFFF;

        /// <summary>
        /// 重建集合，整理并扩容
        /// </summary>
        /// 0
        private void Resize(int size = -1)
        {
            int newSize, newBucketsSize;
            if (size < 0)  //可以改为使用对数式扩容
            {
                var count = _allIndexCount - _freeIndexCount;
                //newSize = checked(count * 2 + 1);
                newSize= (int)(count * 1.0m * (2m + 100m / (count + 100m)));
                newBucketsSize = count;
            }
            else
            {
                newSize = size;
                newBucketsSize =size / 2 + 1;
            }
            var newBuckets = new int[newBucketsSize];
            var newSlots = new Slot[newSize];
            int index = 0;
            for (int i = 0; i < _allIndexCount; i++)
            {
                if (_slots[i].Next == -2)
                    continue;
                int bucket = _slots[i].HashCode % newBucketsSize;
                newSlots[index].HashCode = _slots[i].HashCode;
                newSlots[index].Value = _slots[i].Value;
                newSlots[index].Next = newBuckets[bucket] - 1;
                newBuckets[bucket] = index + 1;
                index++;
            }

            _buckets = newBuckets;
            _slots = newSlots;
            _allIndexCount = index;
            _freeIndexCount = 0;
        }

        private void InitClass()
        {
            _buckets = new int[_capacity];
            _slots = new Slot[_capacity];
            _allIndexCount = 0;
            _freeIndexCount = 0;
            _version = 0;
            _lastVersion = 0;
            _toArray = default;
            _toList = default;
        }
        #endregion //private methods

        #region innerClass

        /// <summary>
        /// 用来包装项的插槽
        /// </summary>
        private struct Slot
        {
            internal int HashCode;

            /// <summary>
            /// 具有相同哈希码余数项的前排邻居的真实索引值
            /// </summary>
            internal int Next;

            internal T Value;
        }


        #endregion //innerClass
    }
}
