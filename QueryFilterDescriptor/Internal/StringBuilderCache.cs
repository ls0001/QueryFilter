using System;
using System.Text;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{
    #region class stringBuilderCache

    internal static class StringBuilderCache
    {
        // The value 360 was chosen in discussion with performance experts as a compromise between using
        // as litle memory per thread as possible and still covering a large part of short-lived
        // StringBuilder creations on the startup path of VS designers.
        private const int maxCapacity = 360;
        private const int defaultCapacity = 16; // == StringBuilder.DefaultCapacity

        // WARNING: We allow diagnostic tools to directly inspect this member (t_cachedInstance).
        // See https://github.com/dotnet/corert/blob/master/Documentation/design-docs/diagnostics/diagnostics-tools-contract.md for more details.
        // Please do not change the type, the name, or the semantic usage of this member without understanding the implication for tools.
        // Get in touch with the diagnostics team if you have questions.
        [ThreadStatic]
        private static StringBuilder? t_cachedInstance;

        /// <summary>Get a StringBuilder for the specified capacity.</summary>
        /// <remarks>If a StringBuilder of an appropriate size is cached, it will be returned and the cache emptied.</remarks>
        public static StringBuilder GetInstance(int capacity = defaultCapacity)
        {
            if (capacity <= maxCapacity)
            {
                var sb = t_cachedInstance;
                if (sb != null)
                {
                    // Avoid stringbuilder block fragmentation by getting a new StringBuilder
                    // when the requested size is larger than the current capacity
                    if (capacity <= sb.Capacity)
                    {
                        t_cachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }

            return new StringBuilder(capacity);
        }

        /// <summary>ToString() the stringbuilder, Release it to the cache, and return the resulting string.</summary>
        public static string GetString(StringBuilder sb)
        {
            string result = sb.ToString();
            if (sb.Capacity <= maxCapacity)
            {
                t_cachedInstance = sb;
            }
            return result;
        }
    }

#endregion blockbase
 
}