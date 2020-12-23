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
            var list = new List<int> { 5, 2, 3, 7, 6, 4, 8, 1, 9 };

            //example input:
            //var list = new List<int> { 3, 8, 9, 1, 2, 5, 4, 6, 7 };

            for (int i = list.Count + 1; i <= 1000000; i++)
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

            int maxElementValue = list.Max();

            //compelte the circle
            first.Previous = lastCreated;
            lastCreated.Next = first;

            Element current = first;

            for (long i = 0; i < 10000000; i++)
            {
                var currentCup = current.Id;

                //Console.WriteLine($"Round {i + 1}");
                //Console.WriteLine(string.Join(" ", list));
                //Console.WriteLine($"Current cup: {currentCup}");

                var pickup1 = current.Next;
                var pickup2 = current.Next.Next;
                var pickup3 = current.Next.Next.Next;

                //remove them by simply linking them out
                current.Next = pickup3.Next;
                pickup3.Previous = current;

                //Console.WriteLine($"Pick up: {pickup1}, {pickup2}, {pickup3}");

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

                //Console.WriteLine($"Destination: {destinationCupLabel}");
                //Console.WriteLine();

                var destination = lookup[destinationCupLabel];

                var originalNext = destination.Next;
                destination.Next = pickup1;
                pickup1.Previous = destination;

                pickup3.Next = originalNext;
                originalNext.Previous = pickup3;

                current = current.Next;
            }

            // final printout Part 1
            var startIndex = lookup[1];
            //string result = string.Empty;

            //for (var element = startIndex.Next; element != startIndex; element = element.Next)
            //{
            //    result += element.Id;
            //}

            //Console.WriteLine($"Part 1: {result}");

            // final printout Part 2
            var result = startIndex.Next.Id * startIndex.Next.Next.Id;

            Console.WriteLine($"Part 2: {startIndex.Next.Id} * {startIndex.Next.Next.Id} = {(long)startIndex.Next.Id * (long)startIndex.Next.Next.Id}");
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
