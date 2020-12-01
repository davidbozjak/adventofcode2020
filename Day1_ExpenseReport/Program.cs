using SantasToolbox;
using System;
using System.Collections.Generic;

namespace Day1_ExpenseReport
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<int>("Input.txt", int.TryParse);

            var inputs = new List<int>();

            while (inputProvider.MoveNext())
            {
                int expense = inputProvider.Current;

                inputs.Add(expense);
            }

            // Part 1

            int sumTo = 2020;

            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 0; j < inputs.Count; j++)
                {
                    if (i == j) continue;

                    if (inputs[i] + inputs[j] == sumTo)
                    {
                        var product = inputs[i] * inputs[j];

                        Console.WriteLine($"Product of {inputs[i]} and {inputs[j]} = {product}");
                    }
                }
            }

            // Part 2

            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 0; j < inputs.Count; j++)
                {
                    if (i == j) continue;

                    for (int k = 0; k < inputs.Count; k++)
                    {
                        if (j == k) continue;

                        if (inputs[i] + inputs[j] + inputs[k] == sumTo)
                        {
                            var product = inputs[i] * inputs[j] * inputs[k];

                            Console.WriteLine($"Product of {inputs[i]} and {inputs[j]} and {inputs[k]} = {product}");
                        }
                    }
                }
            }
        }
    }
}
