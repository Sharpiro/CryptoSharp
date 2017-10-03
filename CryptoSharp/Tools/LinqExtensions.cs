using System.Collections.Generic;
using JetBrains.Annotations;

namespace System.Linq
{
    public static class LinqExtensions
    {
        [NotNull]
        public static string StringJoin([NotNull]this IEnumerable<string> enumerable, [NotNull]string seperator = "") => string.Join(seperator, enumerable);
    }
}