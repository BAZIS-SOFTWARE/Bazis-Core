using System;
using System.Collections.Generic;

namespace SurfaceMesher
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> source, bool isRing)
        {
            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            var head = enumerator.Current;
            var first = head;
            T second;
            while (enumerator.MoveNext())
            {
                second = enumerator.Current;
                yield return Tuple.Create(first, second);
                first = second;
            }
            if(isRing)
            {
                yield return Tuple.Create(first, head);
            }
        }
    }
}
