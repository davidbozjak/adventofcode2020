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

            int countPart1 = input.Count(w => targetRule.IsMatch(w, 0, out int length) && length == w.Length);
            Console.WriteLine($"Part 1: Inputs that matches rule 0: {countPart1}");
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

            public Rule (string input)
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

            public bool IsMatch(string input, int startIndex, out int charsUsed)
            {
                if (this.singleLetterToMatch != null)
                {
                    charsUsed = 1;
                    return input.Length > startIndex && this.singleLetterToMatch == input[startIndex];
                }
                else if (this.possibleSubrules != null)
                {
                    charsUsed = 0;

                    foreach (var ruleList in this.possibleSubrules)
                    {
                        var rules = ruleList.Select(w => ruleDatabase[w]).ToList();
                        charsUsed = 0;

                        bool matchesSublist = true;
                        int innerStartIndex = startIndex;

                        for (int i = 0; i < rules.Count; i++)
                        {
                            var match = rules[i].IsMatch(input, innerStartIndex, out int count);

                            if (!match)
                            {
                                matchesSublist = false;
                                break;
                            }
                            
                            charsUsed += count;
                            innerStartIndex += count;
                        }

                        if (matchesSublist) return true;
                    }

                    return false;
                }
                else throw new Exception();
            }
        }
    }
}
