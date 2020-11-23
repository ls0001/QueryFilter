using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sag.Data.Common.Query;

namespace UnitTestQueryExpression
{
    class Class1<T> where T:struct,Enum
    {
        public void testEnum()
        {
            var v = 1;

            QueryLogical c = (QueryLogical)Unsafe.As<int, QueryLogical>(ref v);
            
           

        }
    }
}
