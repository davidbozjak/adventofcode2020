using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Day15_TBN
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> input = new List<int>() { 14, 3, 1, 0, 9, 5};

            Console.WriteLine($"Part 1: Turn {2020}: {MemoryForTurn(input.GetEnumerator(), 2020)}");
            Console.WriteLine($"Part 2: Turn {30000000}: {MemoryForTurn(input.GetEnumerator(), 30000000)}");
        }

        private static int MemoryForTurn(IEnumerator<int> input, int turn)
        {
            var numberSpokenAt = new Dictionary<int, List<int>>();

            int lastSpokenNumber = -1;
            for (int i = 0; i < turn; i++)
            {
                if (input.MoveNext())
                {
                    lastSpokenNumber = input.Current;
                }
                else
                {
                    if (numberSpokenAt[lastSpokenNumber].Count == 1)
                    {
                        lastSpokenNumber = 0;
                    }
                    else
                    {
                        var list = numberSpokenAt[lastSpokenNumber];
                        lastSpokenNumber = list[^1] - list[^2];
                    }
                }

                if (!numberSpokenAt.ContainsKey(lastSpokenNumber))
                {
                    numberSpokenAt.Add(lastSpokenNumber, new List<int>());
                }

                numberSpokenAt[lastSpokenNumber].Add(i);
            }

            return lastSpokenNumber;
        }
    }
}
