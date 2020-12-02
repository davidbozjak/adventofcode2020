using SantasToolbox;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day2_PasswordPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1();

            Part2();
        }

        static private void Part1()
        {
            using var inputProvider = new InputProvider<PasswordPolicy>("Input.txt", ParseString);

            var inputs = inputProvider.ToList();

            var count = inputs.Where(w => w.PasswordComplies(w.ExamplePassword)).Count();

            Console.WriteLine($"{count} password comply with initial polciy");
        }

        static private void Part2()
        {
            using var inputProvider = new InputProvider<NewPasswordPolicy>("Input.txt", ParseString);

            var inputs = inputProvider.ToList();

            var count = inputs.Where(w => w.PasswordComplies(w.ExamplePassword)).Count();

            Console.WriteLine($"{count} password comply with the new polciy");
        }

        static bool ParseString(string? value, out PasswordPolicy output)
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

        static bool ParseString(string? value, out NewPasswordPolicy output)
        {
            output = null;

            if (string.IsNullOrWhiteSpace(value)) return false;

            try
            {
                output = new NewPasswordPolicy(value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        class PasswordPolicy
        {
            public int MinRepetition { get; }
            public int MaxRepetition { get; }
            public char RequiredChar { get; }
            public string ExamplePassword { get; }

            public PasswordPolicy(string input)
            {
                var numRegex = new Regex(@"\d+");
                var numMatches = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

                this.MinRepetition = numMatches[0];
                this.MaxRepetition = numMatches[1];

                var charRegex = new Regex(@"[a-z]");
                this.RequiredChar = charRegex.Matches(input).Select(w => w.Value[0]).First();

                this.ExamplePassword = input.Substring(input.IndexOf(':') + 2);
            }

            public bool PasswordComplies(string input)
            {
                var count = input.Count(w => w == this.RequiredChar);

                return count >= MinRepetition && count <= MaxRepetition;
            }
        }

        class NewPasswordPolicy
        {
            public int MinRepetition { get; }
            public int MaxRepetition { get; }
            public char RequiredChar { get; }
            public string ExamplePassword { get; }

            public NewPasswordPolicy(string input)
            {
                var numRegex = new Regex(@"\d+");
                var numMatches = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

                this.MinRepetition = numMatches[0];
                this.MaxRepetition = numMatches[1];

                var charRegex = new Regex(@"[a-z]");
                this.RequiredChar = charRegex.Matches(input).Select(w => w.Value[0]).First();

                this.ExamplePassword = input.Substring(input.IndexOf(':') + 2);
            }

            public bool PasswordComplies(string input)
            {
                return (input[MinRepetition - 1] == this.RequiredChar) ^ (input[MaxRepetition - 1] == this.RequiredChar);
            }
        }
    }
}
