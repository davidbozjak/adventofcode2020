using System.Collections.Generic;
using System.Linq;

namespace SantasToolbox
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Provides all different sublists of k elements from enumerable
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}
