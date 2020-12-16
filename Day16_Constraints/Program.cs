using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Day16_Constraints
{
    class Program
    {
        private static readonly Regex numRegex = new Regex(@"\d+");

        static void Main(string[] args)
        {
            //Note: for todays input we need to manually remove the empty lines. Consider improving later.
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var input = inputProvider.ToList();

            var constraints = new List<Constraint>();

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].StartsWith("your ticket:")) break;

                constraints.Add(new Constraint(input[i]));
            }

            var nearbyTickets = new List<List<int>>();
            var invalidTickets = new List<List<int>>();
            var invalidValues = new List<int>();

            var myTicket = numRegex.Matches(input[constraints.Count + 1]).Select(w => int.Parse(w.Value)).ToList();

            for (int i = constraints.Count + 3; i < input.Count; i++)
            {
                var numbers = numRegex.Matches(input[i]).Select(w => int.Parse(w.Value)).ToList();

                nearbyTickets.Add(numbers);

                bool matchesAny = true;
                for (int j = 0; j < numbers.Count; j++)
                {
                    if (!constraints.Any(w => w.ValueMatchesAnyRange(numbers[j])))
                    {
                        matchesAny = false;
                        invalidValues.Add(numbers[j]);
                        break;
                    }
                }
                
                if (!matchesAny)
                {
                    invalidTickets.Add(numbers);
                }
            }

            Console.WriteLine($"Part 1: Sum of all invlaid tickets: {invalidValues.Sum()}");

            invalidTickets.ForEach(w => nearbyTickets.Remove(w));

            var columns = Enumerable.Range(0, constraints.Count).ToList();

            var constraitnsToFind = constraints.ToList();

            while (constraitnsToFind.Count > 0)
            {
                for (int i = 0; i < constraitnsToFind.Count; i++)
                {
                    var possibleColumns = new List<int>();
                    var constraint = constraitnsToFind[i];

                    for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                    {
                        int column = columns[columnIndex];

                        var exampleValues = nearbyTickets.Select(w => w[column]);

                        if (exampleValues.All(w => constraint.ValueMatchesAnyRange(w)))
                        {
                            possibleColumns.Add(column);
                        }
                    }

                    if (possibleColumns.Count == 1)
                    {
                        constraint.IndexInInput = possibleColumns[0];
                        columns.Remove(possibleColumns[0]);
                        constraitnsToFind.Remove(constraint);
                    }
                }
            }

            long multiplicationOfMyDepartureValues = 1;

            for (int i = 0; i < constraints.Count; i++)
            {
                var constraint = constraints[i];

                if (!constraint.Name.StartsWith("departure")) continue;

                multiplicationOfMyDepartureValues *= myTicket[constraint.IndexInInput];
            }

            Console.WriteLine($"Part 2: {multiplicationOfMyDepartureValues}");
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }

        class Constraint
        {
            public string Name { get; }

            public Range[] Ranges { get; }

            public int IndexInInput { get; set; }

            public Constraint(string input)
            {
                var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();
                var ranges = new List<Range>();

                for (int i = 0; i < numbers.Count; i+= 2)
                {
                    ranges.Add(new Range(numbers[i], numbers[i + 1]));
                }

                this.Ranges = ranges.ToArray();
                this.Name = input[..input.IndexOf(':')];

                this.IndexInInput = -1;
            }

            public bool ValueMatchesAnyRange(int value)
            {
                return this.Ranges.Any(w => w.IsValueWithinRange(value));
            }

            public class Range
            {
                int MinValue { get; }
                int MaxValue { get; }

                public Range(int min, int max)
                {
                    this.MinValue = min;
                    this.MaxValue = max;
                }

                public bool IsValueWithinRange(int value)
                {
                    return value >= MinValue && value <= MaxValue;
                }
            }
        }
    }
}
