using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23_CircularCrabs
{
    class Program
    {
        static void Main(string[] args)
        {
            //real input:
            //var list = new List<int> { 5, 2, 3, 7, 6, 4, 8, 1, 9 };

            //example input:
            var list = new List<int> { 3, 8, 9, 1, 2, 5, 4, 6, 7 };

            int currentIndex = 0;

            for (int i = 0; i < 100; i++)
            {
                var currentCup = list[TransformIndex(currentIndex)];
                //Console.WriteLine($"Round {i + 1}");
                //Console.WriteLine(string.Join(" ", list));
                //Console.WriteLine($"Current cup: {currentCup}");
                
                var pickup1 = list[TransformIndex(currentIndex + 1)];
                var pickup2 = list[TransformIndex(currentIndex + 2)];
                var pickup3 = list[TransformIndex(currentIndex + 3)];

                list.Remove(pickup1);
                list.Remove(pickup2);
                list.Remove(pickup3);

                //Console.WriteLine($"Pick up: {pickup1}, {pickup2}, {pickup3}");

                var destinationCupLabel = currentCup - 1;

                while (!list.Contains(destinationCupLabel))
                {
                    destinationCupLabel--;

                    if (destinationCupLabel < 0)
                    {
                        destinationCupLabel = list.Max();
                    }
                }

                //Console.WriteLine($"Destination: {destinationCupLabel}");
                //Console.WriteLine();

                var destinationIndex = list.FindIndex(w => w == destinationCupLabel);

                list.Insert(TransformIndexInsert(destinationIndex + 1), pickup1);
                list.Insert(TransformIndexInsert(destinationIndex + 2), pickup2);
                list.Insert(TransformIndexInsert(destinationIndex + 3), pickup3);

                currentIndex = TransformIndex(list.FindIndex(w => w == currentCup) + 1);
            }

            // final printout Part 1
            var startIndex = list.FindIndex(w => w == 1);
            string result = string.Empty;

            for (int i = 0; i < list.Count - 1; i++)
            {
                result += list[TransformIndex(i + startIndex + 1)];
            }

            Console.WriteLine($"Part 1: {result}");

            int TransformIndex(int index)
            {
                while (index < 0) index += list.Count;
                while (index >= list.Count) index -= list.Count;

                return index;
            }

            int TransformIndexInsert(int index)
            {
                while (index < 0) index += list.Count;
                while (index > list.Count) index -= list.Count;

                return index;
            }
        }
    }
}
