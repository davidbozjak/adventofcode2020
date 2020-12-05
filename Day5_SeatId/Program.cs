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

            var inputs = inputProvider;

            var orderedSeats = inputs.Select(w => GetSeatId(w)).OrderBy(w => w).ToList();

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
            string rowString = input[..7];
            string seatString = input[7..];

            rowString = rowString
                .Replace('F', '0')
                .Replace('B', '1');

            int row = Convert.ToInt32(rowString, 2);

            seatString = seatString
                .Replace('R', '1')
                .Replace('L', '0');

            int seat = Convert.ToInt32(seatString, 2);

            int seatId = row * 8 + seat;

            return seatId;
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }
    }
}
