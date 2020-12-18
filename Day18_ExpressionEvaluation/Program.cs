using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day18_ExpressionEvaluation
{
    class Program
    {
        private static readonly Regex numRegex = new Regex(@"-?\d+");
        private static readonly Regex operatorsRegex = new Regex(@"[\+\*]");

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var input = inputProvider.ToList();

            long sumLeftToRight = input.Sum(w => EvaluateExpressionLeftToRight(w));

            Console.WriteLine($"Part 1: {sumLeftToRight}");

            long sumAdditionFirst = input.Sum(w => EvaluateExpressionAdditionFirst(w));

            Console.WriteLine($"Part 2: {sumAdditionFirst}");
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }

        static long EvaluateExpressionLeftToRight(string input)
        {
            string? parenthesisStr = LongestParenthesisOrNull(input);

            while (parenthesisStr != null)
            {
                var value = EvaluateExpressionLeftToRight(parenthesisStr[1..^1]);
                input = input.Replace(parenthesisStr, value.ToString());

                parenthesisStr = LongestParenthesisOrNull(input);
            }

            var numbers = numRegex.Matches(input).Select(w => long.Parse(w.Value)).ToList();
            var operators = operatorsRegex.Matches(input).Select(w => w.Value).ToList();

            int numIndex = 0;
            long result = numbers[numIndex++];

            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == "+")
                {
                    result += numbers[numIndex++];
                }
                else if (operators[i] == "*")
                {
                    result *= numbers[numIndex++];
                }
                else throw new Exception();
            }

            return result;
        }

        static long EvaluateExpressionAdditionFirst(string input)
        {
            string? parenthesisStr = LongestParenthesisOrNull(input);

            while (parenthesisStr != null)
            {
                var value = EvaluateExpressionAdditionFirst(parenthesisStr[1..^1]);
                input = input.Replace(parenthesisStr, value.ToString());

                parenthesisStr = LongestParenthesisOrNull(input);
            }

            var numbers = numRegex.Matches(input).Select(w => long.Parse(w.Value)).ToList();
            var operators = operatorsRegex.Matches(input).Select(w => w.Value).ToList();

            while (operators.Contains("+"))
            {
                var index = operators.IndexOf("+");
                operators.RemoveAt(index);

                var firstOperandIndex = index;
                var secondOperandIndex = firstOperandIndex + 1;

                var firstOperand = numbers[firstOperandIndex];
                var secondOperand = numbers[secondOperandIndex];

                numbers.RemoveAt(secondOperandIndex);
                numbers.RemoveAt(firstOperandIndex);
                numbers.Insert(firstOperandIndex, firstOperand + secondOperand);
            }

            long result = 1;
            foreach (var number in numbers)
            {
                result *= number;
            }

            return result;
        }

        static string? LongestParenthesisOrNull(string input)
        {
            if (input.IndexOf("(") == -1) return null;
            if (input.IndexOf(")") == -1) return null;

            var list = new List<string>();

            int startIndex = input.IndexOf("(");
            int level = 1;

            for (int i = startIndex + 1; i < input.Length; i++)
            {
                if (input[i] == ')')
                {
                    level--;

                    if (level == 0)
                    {
                        list.Add(input[startIndex..(i + 1)]);
                    }
                }
                else if (input[i] == '(')
                {
                    level++;

                    if (level == 1)
                    {
                        startIndex = i;
                    }
                }
            }

            return list.OrderByDescending(w => w.Length).First();
        }
    }
}
