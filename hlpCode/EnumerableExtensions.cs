using System.Collections.Generic;
using System.Linq;


namespace Helpers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T item, int position)> AppendOrdinal<T>(this IEnumerable<T> sequence) => sequence.Select((item, index) => (item, index));
    }
}
