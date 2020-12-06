using SantasToolbox;
using System;
using System.Linq;

namespace Day5_SeatId
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var orderedSeats = inputProvider.Select(w => GetSeatId(w)).OrderBy(w => w).ToList();

            Console.WriteLine($"Part1: Max Seat Id: {orderedSeats.Last()}");

            for (int i = 1, min = orderedSeats[0]; i < orderedSeats.Count; i++, min++)
            {
                if (orderedSeats[i] != min + 1)
                {
                    Console.WriteLine($"Part 2: Santas seat is {min + 1}");
                    break;
                }
            }
        }

        private static int GetSeatId(string input)
        {
            var binaryString = input
                .Replace('F', '0')
                .Replace('B', '1')
                .Replace('L', '0')
                .Replace('R', '1');

            return Convert.ToInt32(binaryString, 2);
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }
    }
}
