using System.Collections.Generic;
using System.Linq;

namespace SantasToolbox
{
    public static class ListExtenstions
    {
        public static bool AddIfNotNull<T>(this List<T> list, T? element)
            where T:class
        {
            if (element == null)
            {
                return false;
            }

            list.Add(element);
            return true;
        }

        public static IEnumerable<IList<T>> PermuteList<T>(this IList<T> sequence)
        {
            return Permute(sequence, 0, sequence.Count);

            static IEnumerable<IList<T>> Permute(IList<T> sequence, int k, int m)
            {
                if (k == m)
                {
                    yield return sequence;
                }
                else
                {
                    for (int i = k; i < m; i++)
                    {
                        SwapPlaces(sequence, k, i);

                        foreach (var newSquence in Permute(sequence, k + 1, m))
                        {
                            yield return newSquence;
                        }

                        SwapPlaces(sequence, k, i);
                    }
                }
            }

            static void SwapPlaces(IList<T> sequence, int indexA, int indexB)
            {
                T temp = sequence[indexA];
                sequence[indexA] = sequence[indexB];
                sequence[indexB] = temp;
            }
        }

        public static IEnumerable<IList<T>> GetAllOrdersOfList<T>(this IList<T> sequence)
        {
            if (sequence.Count == 1) yield return sequence;

            foreach (var element in sequence)
            {
                var list = new List<T>();
                var listWithoutElement = sequence.ToList();
                listWithoutElement.Remove(element);
                list.Add(element);

                foreach (var subsequence in listWithoutElement.GetAllOrdersOfList())
                {
                    var copyList = list.ToList();

                    copyList.AddRange(subsequence);
                    yield return copyList;
                }

            }
        }
    }
}
