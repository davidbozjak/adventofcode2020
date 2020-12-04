using System;
using System.Collections.Generic;
using System.Linq;
using SantasToolbox;

namespace Day3_Trajectory
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<RepeatingRow?>("Input.txt", RepeatingRow.Parse);

            var map = inputProvider.Where(w => w != null).Cast<RepeatingRow>().ToList();

            Part1(map);

            Part2(map);
        }

        static void Part1(IList<RepeatingRow> map)
        {
            int noOfTrees = TraverseMapWithMethod(map, x => x + 3, y => y + 1);

            Console.WriteLine($"No of trees on the way: {noOfTrees}");
        }

        static void Part2(IList<RepeatingRow> map)
        {
            int case1 = TraverseMapWithMethod(map, x => x + 1, y => y + 1);
            int case2 = TraverseMapWithMethod(map, x => x + 3, y => y + 1);
            int case3 = TraverseMapWithMethod(map, x => x + 5, y => y + 1);
            int case4 = TraverseMapWithMethod(map, x => x + 7, y => y + 1);
            int case5 = TraverseMapWithMethod(map, x => x + 1, y => y + 2);

            var finalState = case1 * case2 * case3 * case4 * case5;

            Console.WriteLine($"No of trees on the way: {finalState}");
        }

        static int TraverseMapWithMethod(IList<RepeatingRow> map, Func<int, int> xFunc, Func<int, int> yFunc)
        {
            int noOfTrees = 0;

            for (int y = 0, x = 0; y < map.Count; x = xFunc(x), y = yFunc(y))
            {
                if (map[y][x] == '#') noOfTrees++;
            }

            return noOfTrees;
        }

        class RepeatingRow
        {
            private readonly string row;

            public char this[int index]
            {
                get => this.row[index % row.Length];
            }

            public RepeatingRow (string input)
            {
                this.row = input;
            }

            public static bool Parse(string? input, out RepeatingRow? value)
            {
                value = null;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    value = new RepeatingRow(input);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
