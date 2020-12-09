using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day9_Sublists
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<int>("Input.txt", int.TryParse);

            int queueSize = 25;
            
            int keyNumber = GetKeyNumber(inputProvider, queueSize);
            Console.WriteLine($"Part 1: Can't construct number {keyNumber}");

            inputProvider.Reset();
            var inputs = inputProvider.ToList();

            var list = GetListThatSums(keyNumber, inputs);

            Console.WriteLine($"Part 2: Min and max of the list sum to: {list.Min() + list.Max()}");
            Console.WriteLine("Whole list:");
            Console.WriteLine(string.Join(Environment.NewLine, list));
        }

        private static int GetKeyNumber(IEnumerator<int> inputProvider, int queueSize)
        {
            var queue = new Queue<int>();

            for (int i = 0, number = inputProvider.Current; inputProvider.MoveNext(); i++, number = inputProvider.Current)
            {
                if (i > queueSize)
                {
                    queue.Dequeue();

                    if (!CanConstructNumberFromList(number, queue.ToList()))
                    {
                        return number;
                    }
                }

                queue.Enqueue(number);
            }

            return -1;
        }

        private static IList<int> GetListThatSums(int numberToFind, IList<int> inputs)
        {
            for (int i = 2; i < inputs.Count; i++)
            {
                for (int lengthOfList = 1; lengthOfList < i; lengthOfList++)
                {
                    var list = new List<int>();

                    for (int k = lengthOfList; k > 0; k--)
                    {
                        list.Add(inputs[i - k]);
                    }

                    int sum = list.Sum();

                    if (sum == numberToFind) return list;
                    if (sum > numberToFind) break;
                }
            }

            return new List<int>();
        }

        static bool CanConstructNumberFromList(int number, IList<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i] + list[j] == number) return true;
                }
            }

            return false;
        }
    }
}
