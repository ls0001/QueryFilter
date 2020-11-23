using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sag.Data.Common.Query.Internal
{

    /// <summary>
    /// 集合内部列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[DebuggerTypeProxy(typeof(QueryExprInternalListDebugView<>))]
    [DebuggerDisplay("InnerList:[{_count}] <{typeof(T).Name,nq}>")]
    internal class InternalList<T> : IEnumerable<T>//, ICollection<T>
    {
        #region 变量

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        const int defaultCapacity = 2;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _count = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        T[] defaultItems = new T[defaultCapacity];
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        T[] _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _version;

        #endregion //变量


        #region 属性和索引 
        internal int Count => _count;

        public bool IsReadOnly { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal T this[int index]
        {
            get
            {
                if (index >= 0 && index < _count)
                    return _items[index];

                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < _count)
                    _items[index] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        #endregion //属性和索引 


        #region 构造

        internal InternalList()
        {
            _items = defaultItems;
        }

        internal InternalList(int capacity)
        {
            if (capacity > defaultCapacity)
                _items = new T[capacity];
            else
                _items = defaultItems;
        }

        internal InternalList(IEnumerable<T> collection)
        {
            if (collection is ICollection<T> c)
            {
                var count = c.Count;
                if (count != 0)
                {
                    _items = count > defaultCapacity ? new T[count] : defaultItems;
                    c.CopyTo(_items, 0);
                    _count = count;
                }
                else
                {
                    _items = defaultItems;
                }
            }
            else
            {
                _items = defaultItems;
                using (IEnumerator<T> en = collection!.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        #endregion //构造


        internal void Add(T item)
        {

            if (_count >= _items.Length)
            {
                int newSize;
                if (_count <= 4 * defaultCapacity)
                    newSize = _count * 2;
                else
                    newSize = (int)(_count * (1 + Math.Sqrt(Math.Log(32, _count / 2 + 2))));

                var newArr = new T[(int)newSize];
                Array.Copy(_items, 0, newArr, 0, _count);
                _items = newArr;
            }

            _items[_count] = item;
            _count++;
            _version++;

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void RemoveAt(int id)
        {
            _count--;
            Array.Copy(_items, id + 1, _items, id, _count - id);
            _items[_count] = default!;
            _version++;
        }

        internal int IndexOf(T value)
        {
            return Array.IndexOf(_items, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count">默认=0,表示从index起后面的全部</param>
        /// <returns></returns>
        internal T[] ToArray(int index, int count = 0)
        {
            if (count == 0) count = _count - index;//_items.Length-index;
            if (index < 0 || count < 0 || count>_count||(index>_count-1 &&index!=0))
                throw new IndexOutOfRangeException(MsgStrings.IndexOutOfRange);
            var arr = new T[count];
            Array.Copy(_items, index, arr, 0, count);
            return arr;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void Clear()
        {
            //是引用类型或包含引用的值类型
            if (_count > 0 && RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_items, 0, _count);
            }
            _count = 0;
            _version = 0;
        }

        public override string ToString()
        {
            return "QueryInternalList";
        }


        #region 枚举接口

        public Enumerator GetEnumerator()
            => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => new Enumerator(this);


        internal struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly InternalList<T> _members;
            private int _index;
            private readonly int _version;
            private T _current;

            internal Enumerator(InternalList<T> members)
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
                    _current = _members._items[_index];
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

            public T Current => _current!;

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

                #endregion //枚举接口

  /*
                #region ICollection 接口
                
                int ICollection<T>.Count { get; }
                void ICollection<T>.Add(T item) => Add(item);
                void ICollection<T>.Clear() => Clear();
                public bool Contains(T item) => IndexOf(item) >= 0;

                /// <summary>
                /// 复制不下的,丢掉
                /// </summary>
                /// <param name="array"></param>
                /// <param name="arrayIndex"></param>
                public void CopyTo(T[] array, int arrayIndex)
                {
                    var count = _items.Length - arrayIndex;
                    //if (count < 0)
                    //    throw new IndexOutOfRangeException(MsgStrings.IndexOutOfRange);
                    Array.Copy(_items, arrayIndex, array, 0, count > array.Length ? array.Length : count);
                }
                public bool Remove(T item)
                {
                    var index = IndexOf(item);
                    if (index < 0) return false;
                    RemoveAt(index);
                    return true;
                }

                #endregion ICollection 
                */
    }

    /// <summary>
    /// 桶里的链,它链到目标元素,用作记录目标实体与桶之间中转器
    /// </summary>
    internal struct Link
    {
        internal uint hashCode;
        internal int next;            //在链列表中与本链相邻连的链索引
        internal InternalList<int> members;//在目标值列表中的目标元素索引
    }


}
