using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14_Masking
{
    class Program
    {
        private static readonly Regex numRegex = new Regex(@"-?\d+");

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var input = inputProvider.ToList();
            
            Part1(input);

            Part2(input);
        }

        private static void Part1(List<string> input)
        {
            long andMaskValue = 0;
            long orMaskValue = 0;

            Dictionary<long, long> memory = new Dictionary<long, long>();

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].StartsWith("mask = "))
                {
                    (andMaskValue, orMaskValue) = GetMaskValues(input[i]);
                    continue;
                }

                var values = numRegex.Matches(input[i]).Select(w => int.Parse(w.Value)).ToList();

                long address = values[0];
                long valueToWrite = values[1];

                valueToWrite |= orMaskValue;
                valueToWrite &= andMaskValue;

                if (valueToWrite != 0)
                {
                    memory[address] = valueToWrite;
                }
                else if (memory.ContainsKey(address))
                {
                    memory.Remove(address);
                }
            }

            Console.WriteLine($"Part 1: Sum of all non-zero values: {memory.Values.Sum()}");

            static (long andMaskValue, long orMaskValue) GetMaskValues(string maskInputString)
            {
                string mask = maskInputString[(maskInputString.LastIndexOf("=") + 2)..];

                long andMaskValue = 0;
                long orMaskValue = 0;

                for (int i = mask.Length - 1; i >= 0; i--)
                {
                    long bit = (long)Math.Pow(2, mask.Length - 1 - i);

                    if (mask[i] == '1')
                    {
                        orMaskValue += bit;
                    }

                    if (mask[i] != '0')
                    {
                        andMaskValue += bit;
                    }
                }

                return (andMaskValue, orMaskValue);
            }
        }

        private static void Part2(List<string> input)
        {
            Dictionary<long, long> memory = new Dictionary<long, long>();

            string mask = string.Empty;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].StartsWith("mask = "))
                {
                    mask = input[i][(input[i].LastIndexOf("=") + 2)..];
                    continue;
                }

                var values = numRegex.Matches(input[i]).Select(w => int.Parse(w.Value)).ToList();

                long initialAddress = values[0];
                long valueToWrite = values[1];

                foreach (var address in GetAddressesToWrite(mask, initialAddress))
                {
                    if (valueToWrite != 0)
                    {
                        memory[address] = valueToWrite;
                    }
                    else if (memory.ContainsKey(address))
                    {
                        memory.Remove(address);
                    }
                }
            }

            Console.WriteLine($"Part 2: Sum of all non-zero values: {memory.Values.Sum()}");

            static IList<long> GetAddressesToWrite(string mask, long address)
            {
                var indexToSuperimpose = new List<int>();

                for (int i = mask.Length - 1; i >= 0; i--)
                {
                    long maskToOr = (long)Math.Pow(2, mask.Length - 1 - i);

                    if (mask[i] == '1')
                    {
                        address |= maskToOr;
                    }
                    else if(mask[i] == 'X')
                    {
                        indexToSuperimpose.Add(i);
                    }
                }

                var addresses = new List<long>() { address };

                foreach (var index in indexToSuperimpose)
                {
                    long maskToOr = (long)Math.Pow(2, mask.Length - 1 - index);
                    long maskToAnd = long.MaxValue - (long)Math.Pow(2, mask.Length - 1 - index);

                    var evaluatedAddresses = new List<long>();

                    foreach (var a in addresses)
                    {
                        evaluatedAddresses.Add(a | maskToOr);
                        evaluatedAddresses.Add(a & maskToAnd);
                    }

                    addresses = evaluatedAddresses;
                }

                return addresses;
            }
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }
    }
}
