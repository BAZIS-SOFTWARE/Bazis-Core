using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extensions
{
    public static class EnumerableExtentions
    {
        /// <summary>
        /// Bigrams selector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// Перебираемое множество
        /// <param name="returFirstObj"></param>
        /// Возвращает пару содержащую последний и первый объект в коллекции в конце перебора
        /// <returns></returns>
        public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> items, bool returFirstObj)
        {
            var enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            var head = items.First();
            while (enumerator.MoveNext())
            {
                var second = enumerator.Current;
                yield return Tuple.Create(first, second);
                first = second;
            }

            if (returFirstObj)
                yield return Tuple.Create(first, head);
        }
    }
}
