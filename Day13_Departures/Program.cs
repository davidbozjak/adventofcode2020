using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day13_Departures
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var input = inputProvider.ToList();
            
            Part1(input);
            Part2(input[1]);
        }

        private static void Part1(IList<string> input)
        {
            int earliestTime = int.Parse(input[0]);
            var busses = input[1].Split(",").Where(w => w != "x").Select(w => int.Parse(w)).ToList();

            int minMinutesLeft = int.MaxValue;
            int nextBus = int.MaxValue;

            foreach (var bus in busses)
            {
                int lastDeparture = earliestTime % bus;
                int minutesLeft = bus - lastDeparture;

                if (minutesLeft < minMinutesLeft)
                {
                    minMinutesLeft = minutesLeft;
                    nextBus = bus;
                }
            }

            Console.WriteLine($"Part1: Bus {nextBus} departing in {minMinutesLeft} minutes. Result: {nextBus * minMinutesLeft}");
        }

        private static void Part2(string busList)
        {
            var busses = busList.Split(",").ToList();

            var constraints = new (int frequency, int offset)[busses.Count(w => w != "x")];

            int noConstraint = 0;
            for (int i = 0; i < busses.Count; i++)
            {
                if (busses[i] == "x") continue;

                constraints[noConstraint++] = (int.Parse(busses[i]), i);
            }

            long baseFactor = constraints[0].frequency;

            long factorIncrease = 1;
            long minFactor = 1;
            for (int i = 1; i < constraints.Length; i++)
            {
                var constraint = constraints[i];

                var repetitions = new List<long>();

                for (long factor = minFactor; ; factor += factorIncrease)
                {
                    var departure = baseFactor * factor + constraint.offset;

                    if (departure % constraint.frequency == 0)
                    {
                        repetitions.Add(factor);

                        if (repetitions.Count == 2)
                        {
                            minFactor = repetitions[0];
                            factorIncrease = repetitions[1] - repetitions[0];
                            break;
                        }
                    }
                }
            }

            Console.WriteLine($"Part2: {baseFactor * minFactor}");
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }
    }
}
