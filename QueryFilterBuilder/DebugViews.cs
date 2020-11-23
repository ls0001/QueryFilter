using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

    internal sealed class QueryExprInternalListDebugView<T>
    {
        private readonly QueryExprInternalList<T> _collection;
        private T[]? _catchedList;
        public QueryExprInternalListDebugView(QueryExprInternalList<T> collection)
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

    
    internal sealed class QueryPartsCollectionDebugView<Op, Expr> where Op : Enum where Expr:QueryExprBase//,IEquatable<Expr>
    {
        private readonly QueryPartsCollection<Op, Expr> _collection;
        private QueryExprOperatorPair<Op, Expr>[]? _cachedItems;
        public QueryPartsCollectionDebugView(QueryPartsCollection<Op, Expr> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public QueryExprOperatorPair<Op, Expr>[] Items
        {
            get
            {
                return _cachedItems ?? (_cachedItems = _collection.ToArray());

            }
        }

        
    }


    internal sealed class QueryExprOperatorPairDebugView<op, expr> where op : Enum where expr:QueryExprBase
    {
        private readonly QueryExprOperatorPair<op, expr> _pair;

        public QueryExprOperatorPairDebugView(QueryExprOperatorPair<op, expr> pair)
        {
            _pair = pair ?? throw new ArgumentNullException(nameof(pair));
        }

        public op Operator => _pair.Operator;

        public expr Expression => _pair.Expression;

    }

    internal sealed class QueryBlockDebugView<op, item, block> 
        where op : Enum 
        where item : QueryItemBase,IEquatable<item>
        where block : QueryBlockBase<item, block, op>//,IEquatable<QueryBlockBase<item,block,op>>
    {
        private readonly QueryBlockBase<item, block, op> _queryBlock;
        private QueryExprOperatorPair<op,item>[] _cacheditems;
        private QueryExprOperatorPair<op, block>[]_cachedblocks;
        public QueryBlockDebugView(QueryBlockBase<item, block, op> queryBlock)
        {
            _queryBlock = queryBlock;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public QueryExprOperatorPair<op,item>[] Items
            => _cacheditems ?? (_cacheditems = _queryBlock.Items);

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public QueryExprOperatorPair<op, block>[] Blocks
            => _cachedblocks ?? (_cachedblocks = _queryBlock.Blocks);
    }
}
