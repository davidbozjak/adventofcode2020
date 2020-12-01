using SantasToolbox;
using System;
using System.Linq;

namespace Day1_ExpenseReport
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<int>("Input.txt", int.TryParse);

            var inputs = inputProvider.ToList();

            // Part 1

            int sumTo = 2020;

            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = i + 1; j < inputs.Count; j++)
                {
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
                for (int j = i + 1; j < inputs.Count; j++)
                {
                    for (int k =  j + 1; k < inputs.Count; k++)
                    {
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
