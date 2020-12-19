using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day19_Grammar
{
    class Program
    {
        private static readonly Dictionary<int, Rule> ruleDatabase = new Dictionary<int, Rule>();
        private static readonly Regex numRegex = new Regex(@"-?\d+");
        private static readonly Regex singleLetterRegex = new Regex("[a-z]");

        static void Main(string[] args)
        {
            using var ruleProvider = new InputProvider<string>("Rules.txt", GetString);

            foreach (var line in ruleProvider)
            {
                var rule = new Rule(line);
                ruleDatabase.Add(rule.Id, rule);
            }

            var targetRule = ruleDatabase[0];

            using var inputProvider = new InputProvider<string>("Input.txt", GetString);
            var input = inputProvider.ToList();

            int countPart1 = input.Count(w =>
            {
                var match = targetRule.IsMatch(w, 0);
                return match.isMatch && match.charsUsed.Any(ww => ww == w.Length);
            });

            Console.WriteLine($"Part 1: Inputs that matches rule 0: {countPart1}");

            // for part 2 replace two rules
            ruleDatabase[8] = new Rule("8: 42 | 42 8");
            ruleDatabase[11] = new Rule("11: 42 31 | 42 11 31");

            int countPart2 = input.Count(w =>
            {
                var match = targetRule.IsMatch(w, 0);
                return match.isMatch && match.charsUsed.Any(ww => ww == w.Length);
            });

            Console.WriteLine($"Part 2: Inputs that matches rule 0: {countPart2}");
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }

        class Rule
        {
            public int Id { get; }

            private readonly char? singleLetterToMatch;
            private readonly List<List<int>>? possibleSubrules;

            public Rule(string input)
            {
                var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

                this.Id = numbers[0];

                var letter = singleLetterRegex.Match(input);

                if (letter.Success)
                {
                    this.singleLetterToMatch = letter.Value[0];
                }
                else
                {
                    this.possibleSubrules = new List<List<int>>();

                    if (input.Contains("|"))
                    {
                        for (int i = 3; i < input.Length; i++)
                        {
                            var indexOfSeperator = input.IndexOf("|", i);
                            indexOfSeperator = indexOfSeperator < 0 ? input.Length : indexOfSeperator;
                            var subrule = input[i..indexOfSeperator].Trim();

                            this.possibleSubrules.Add(numRegex.Matches(subrule).Select(w => int.Parse(w.Value)).ToList());

                            i = indexOfSeperator;
                        }
                    }
                    else
                    {
                        possibleSubrules.Add(numbers.Skip(1).ToList());
                    }
                }
            }

            public (bool isMatch, IList<int> charsUsed) IsMatch(string input, int startIndex)
            {
                if (this.singleLetterToMatch != null)
                {
                    if (input.Length > startIndex && this.singleLetterToMatch == input[startIndex])
                    {
                        return (true, new[] { 1 });
                    }
                    else
                    {
                        return (false, new[] { 0 });
                    }
                }
                else if (this.possibleSubrules != null)
                {
                    var results = this.possibleSubrules
                        .Select(w => MatchRules(input, startIndex, w.Select(ww => ruleDatabase[ww]).ToList()))
                        .ToList();

                    if (results.Any(w => w.isMatch))
                    {
                        return (true, results.Where(w => w.isMatch).SelectMany(w => w.charsUsed).ToList());
                    }
                    else return (false, new[] { 0 });
                }
                else throw new Exception();
            }

            private (bool isMatch, IList<int> charsUsed) MatchRules(string input, int startIndex, List<Rule> rules)
            {
                bool allMatch = true;
                List<int> charsUsed = new List<int>() { 0 };

                for (int i = 0; i < rules.Count; i++)
                {
                    List<int> startsForNextRule = new List<int>();
                    bool anyMatch = false;
                    while (charsUsed.Count > 0)
                    {
                        var c = charsUsed[0];
                        charsUsed.RemoveAt(0);

                        var result = rules[i].IsMatch(input, startIndex + c);

                        if (!result.isMatch) continue;
                        anyMatch = true;

                        startsForNextRule.AddRange(result.charsUsed.Where(w => w > 0).Select(w => w + c));
                    }
                    
                    if (!anyMatch)
                    {
                        allMatch = false;
                        break;
                    }

                    charsUsed = startsForNextRule;
                }

                if (allMatch) return (true, charsUsed);
                else return (false, new [] { 0 });
            }
        }
    }
}
