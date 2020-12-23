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
            var list = new List<int> { 5, 2, 3, 7, 6, 4, 8, 1, 9 };

            //example input:
            //var list = new List<int> { 3, 8, 9, 1, 2, 5, 4, 6, 7 };

            Part1(list);

            Part2(list);
        }

        static void Part1(IList<int> inputList)
        {
            (Element first, Dictionary<int, Element> lookup) = InitializeCircularList(inputList, 0);

            Run(100, first, lookup);

            var startIndex = lookup[1];
            string result = string.Empty;

            for (var element = startIndex.Next; element != startIndex; element = element.Next)
            {
                result += element.Id;
            }

            Console.WriteLine($"Part 1: {result}");
        }

        static void Part2(IList<int> inputList)
        {
            (Element first, Dictionary<int, Element> lookup) = InitializeCircularList(inputList, 1000000);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Run(10000000, first, lookup);

            stopwatch.Stop();

            var startIndex = lookup[1];
            var result = (long)startIndex.Next.Id * (long)startIndex.Next.Next.Id;

            Console.WriteLine($"Part 2: {startIndex.Next.Id} * {startIndex.Next.Next.Id} = {result}");
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
        }

        static (Element firstElement, Dictionary<int, Element> lookup) InitializeCircularList(IEnumerable<int> initialMembers, int addSequentialUpToAndIncluding)
        {
            var list = new List<int>(initialMembers);

            for (int i = list.Count + 1; i <= addSequentialUpToAndIncluding; i++)
                list.Add(i);

            Dictionary<int, Element> lookup = new Dictionary<int, Element>();

            Element first = new Element(list[0]);
            lookup[list[0]] = first;
            Element lastCreated = first;

            for (int i = 1; i < list.Count; i++)
            {
                Element element = new Element(list[i]);
                lookup[list[i]] = element;

                element.Previous = lastCreated;
                lastCreated.Next = element;

                lastCreated = element;
            }

            //compelte the circle
            first.Previous = lastCreated;
            lastCreated.Next = first;

            return (first, lookup);
        }

        static void Run(int noOfRounds, Element first, Dictionary<int, Element> lookup)
        {
            Element current = first;

            int maxElementValue = lookup.Keys.Max();

            for (long i = 0; i < noOfRounds; i++)
            {
                var currentCup = current.Id;

                var pickup1 = current.Next;
                var pickup2 = current.Next.Next;
                var pickup3 = current.Next.Next.Next;

                //remove them by simply linking them out
                current.Next = pickup3.Next;
                pickup3.Previous = current;

                var destinationCupLabel = currentCup - 1;

                var removedItems = new[] { 0, pickup1.Id, pickup2.Id, pickup3.Id };

                while (removedItems.Contains(destinationCupLabel))
                {
                    destinationCupLabel--;

                    if (destinationCupLabel <= 0)
                    {
                        destinationCupLabel = maxElementValue;
                    }
                }

                var destination = lookup[destinationCupLabel];

                // insert into the list by simply "waving them in"
                var originalNext = destination.Next;
                destination.Next = pickup1;
                pickup1.Previous = destination;

                pickup3.Next = originalNext;
                originalNext.Previous = pickup3;

                current = current.Next;
            }
        }

        class Element
        {
            public int Id { get; }

            public Element Next { get; set; }

            public Element Previous { get; set; }

            public Element(int id)
            {
                this.Id = id;
            }

            public override string ToString()
            {
                return this.Id.ToString();
            }
        }
    }
}
