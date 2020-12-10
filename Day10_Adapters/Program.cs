using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10_TBN
{
    class Program
    {
        static int maxStartIndex = 0;

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

            var noOptions = 1 + MinimizeChain(inputs, 1);

            Console.WriteLine($"Part 2: NoOptions: {noOptions}");

            Console.ReadLine();
            //var chain = MakeChain(inputs);
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

        static long MinimizeChain(IList<int> chain, int startIndex)
        {
            if (startIndex > maxStartIndex)
            {
                Console.WriteLine(startIndex);
                maxStartIndex = startIndex;
            }

            long noOptions = 0;

            for (int i = startIndex; i < chain.Count - 1; i++)
            {
                var tmpList = chain.ToList();
                tmpList.Remove(chain[i]);

                if (IsValidChain(tmpList))
                {
                    //PrintChain(tmpList);
                    noOptions = noOptions + 1 + MinimizeChain(tmpList, i);
                }
            }

            return noOptions;
        }

        static void PrintChain(IList<int> chain)
        {
            Console.WriteLine(string.Join(", ", chain));
        }
    }
}
