using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static string StringJoin(this IEnumerable<string> enumerable, string seperator = "") => string.Join(seperator, enumerable);
    }
}