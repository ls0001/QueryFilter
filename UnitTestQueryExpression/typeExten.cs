using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestQueryExpression
{
    internal static class typeExten
    {

        public static void dump(this object obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
