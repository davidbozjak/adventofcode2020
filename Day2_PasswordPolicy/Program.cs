using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day2_PasswordPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<PasswordPolicy?>("Input.txt", ParseString);

            var inputs = inputProvider.Where(w => w != null).Cast<PasswordPolicy>().ToList();

            Part1(inputs);

            Part2(inputs);
        }

        static private void Part1(IEnumerable<PasswordPolicy> inputs)
        {
            var count = inputs.Where(w => PasswordComplies(w, w.ExamplePassword)).Count();

            Console.WriteLine($"{count} password comply with initial polciy");

            bool PasswordComplies(PasswordPolicy policy, string input)
            {
                var count = input.Count(w => w == policy.RequiredChar);

                return count >= policy.MinRepetition && count <= policy.MaxRepetition;
            }
        }

        static private void Part2(IEnumerable<PasswordPolicy> inputs)
        {
            var count = inputs.Where(w => PasswordComplies(w, w.ExamplePassword)).Count();

            Console.WriteLine($"{count} password comply with the new polciy");

            bool PasswordComplies(PasswordPolicy policy, string input)
            {
                return (input[policy.MinRepetition - 1] == policy.RequiredChar) ^
                    (input[policy.MaxRepetition - 1] == policy.RequiredChar);
            }
        }

        static bool ParseString(string? value, out PasswordPolicy? output)
        {
            output = null;

            if (string.IsNullOrWhiteSpace(value)) return false;

            try
            {
                output = new PasswordPolicy(value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        class PasswordPolicy
        {
            private static readonly Regex numRegex = new Regex(@"\d+");
            private static readonly Regex charRegex = new Regex(@"[a-z]");

            public int MinRepetition { get; }
            public int MaxRepetition { get; }
            public char RequiredChar { get; }
            public string ExamplePassword { get; }

            public PasswordPolicy(string input)
            {
                var numMatches = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

                this.MinRepetition = numMatches[0];
                this.MaxRepetition = numMatches[1];

                this.RequiredChar = charRegex.Matches(input).Select(w => w.Value[0]).First();

                this.ExamplePassword = input[(input.IndexOf(':') + 2)..];
            }
        }
    }
}
