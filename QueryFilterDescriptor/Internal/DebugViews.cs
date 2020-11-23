using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }

    internal sealed class NodeInternalListDebugView<T>
    {
        private readonly InternalList<T> _collection;
        private T[]? _catchedList;
        public NodeInternalListDebugView(InternalList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return _catchedList ?? (_catchedList = _collection.ToArray(0, _collection.Count));
            }
        }
    }

    
    internal sealed class NodeCollectionDebugView<Op, Expr> where Op :struct, Enum where Expr:QueryNode//,IEquatable<Expr>
    {
        private readonly NodeCollection<Op, Expr> _collection;
        private NodeOperatorPair<Op, Expr>[]? _cachedItems;
        public NodeCollectionDebugView(NodeCollection<Op, Expr> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<Op, Expr>[] Items
        {
            get
            {
                return _cachedItems ?? (_cachedItems = _collection.ToArray());

            }
        }

        
    }


    internal sealed class NodeOperatorPairDebugView<op, expr> where op : struct, Enum where expr:QueryNode
    {
        private readonly NodeOperatorPair<op, expr> _pair;

        public NodeOperatorPairDebugView(NodeOperatorPair<op, expr> pair)
        {
            _pair = pair ?? throw new ArgumentNullException(nameof(pair));
        }

        public op Operator => _pair.Operator;

        public expr Expression => _pair.Node;

    }

    internal sealed class NodeBlockDebugView<op, item, block> 
        where op :struct, Enum 
        where item : QueryItem,IEquatable<item>
        where block : QueryBlock<item, block, op>//,IEquatable<QueryBlock<item,block,op>>
    {
        private readonly QueryBlock<item, block, op> _queryBlock;
        private NodeOperatorPair<op,item>[] _cacheditems;
        private NodeOperatorPair<op, block>[]_cachedblocks;
        public NodeBlockDebugView(QueryBlock<item, block, op> queryBlock)
        {
            _queryBlock = queryBlock;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<op,item>[] Items
            => _cacheditems ?? (_cacheditems = _queryBlock.Items);

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<op, block>[] Blocks
            => _cachedblocks ?? (_cachedblocks = _queryBlock.Blocks);
    }
}
