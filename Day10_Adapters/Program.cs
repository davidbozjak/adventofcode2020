using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10_TBN
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<int>("Input.txt", int.TryParse);

            var inputs = inputProvider.OrderBy(w => w).ToList();

            int noOfDiff1 = 0;
            int noOfDiff3 = 0;

            inputs.Insert(0, 0);
            inputs.Add(inputs.Max() + 3);

            for (int i = 0; i < inputs.Count - 1; i++)
            {
                var diff = inputs[i + 1] - inputs[i];
                if (diff == 1) noOfDiff1++;
                else if (diff == 3) noOfDiff3++;
                else if (diff < 1 || diff > 3) throw new Exception();
            }

            Console.WriteLine($"Part 1: Diff1: {noOfDiff1} Diff3: {noOfDiff3} Multiplied: {noOfDiff1 * noOfDiff3}");

            Dictionary<int, long> countForIndex = new Dictionary<int, long>();
            countForIndex.Add(inputs.Count - 2, 1); // only one way to reach the last one, since I added it myself - it is "artificial"

            var noOptions = BuildChain(inputs, 0, countForIndex);

            Console.WriteLine($"Part 2: NoOptions: {noOptions}");
        }

        static long BuildChain(IList<int> adapters, int startIndex, Dictionary<int, long> countForIndex)
        {
            if (countForIndex.ContainsKey(startIndex))
                return countForIndex[startIndex];

            long waysToReach = 0;

            for (int i = 1; i <= 3; i++)
            {
                if (startIndex + i >= adapters.Count) break;

                if (adapters[startIndex + i] - adapters[startIndex] <= 3)
                {
                    waysToReach += BuildChain(adapters, startIndex + i, countForIndex);
                }
            }

            countForIndex[startIndex] = waysToReach;
            return waysToReach;
        }

        static bool IsValidChain(IList<int> chain)
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                var diff = chain[i + 1] - chain[i];

                if (diff > 3) return false;
            }

            return true;
        }
    }
}
