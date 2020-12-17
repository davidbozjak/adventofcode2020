using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Day17_MultiGrid
{
    class Program
    {
        private static UniqueFactory<string, Cell> grid;
        private static bool ignore4d;

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var input = inputProvider.ToList();

            ignore4d = true;
            var countWith3Dimensions = SimulateState(input);
            Console.WriteLine($"Part1: {countWith3Dimensions}");

            ignore4d = false;
            var countWith4Dimensions = SimulateState(input);
            Console.WriteLine($"Part2: {countWith4Dimensions}");
        }

        private static int SimulateState(List<string> input)
        {
            grid = new UniqueFactory<string, Cell>(coordinates => new Cell(coordinates));

            int y = 0;
            foreach (var row in input)
            {
                for (int x = 0; x < row.Length; x++)
                {
                    var key = Cell.FormatCoordinates(x, y, 0, 0);

                    var cell = grid.GetOrCreateInstance(key);

                    cell.Active = row[x] == '#';
                }

                y++;
            }

            for (int generation = 0; generation < 6; generation++)
            {
                foreach (var cell in grid.AllCreatedInstances)
                {
                    cell.EnsureNeighborsExist();
                }

                foreach (var cell in grid.AllCreatedInstances)
                {
                    cell.Evaluate();
                }

                foreach (var cell in grid.AllCreatedInstances)
                {
                    cell.Update();
                }
            }

            var activeCount = grid.AllCreatedInstances.Count(w => w.Active);
            return activeCount;
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }

        class Cell
        {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }
            public int H { get; }

            public string Coordinates => FormatCoordinates(this.X, this.Y, this.Z, this.H);

            public static string FormatCoordinates(int x, int y, int z, int h)
            {
                return $"{x}, {y}, {z}, {h}";
            }

            public bool Active { get; set; }
            private bool activeAfterUpdate;

            private List<Cell>? neighbours;

            public Cell(string coordinates)
            {
                var parts = coordinates.Split(",");

                this.X = int.Parse(parts[0]);
                this.Y = int.Parse(parts[1]);
                this.Z = int.Parse(parts[2]);
                this.H = int.Parse(parts[3]);
                this.Active = false;
            }
            public bool Evaluate()
            {
                this.EnsureNeighborsExist();

                var noOfOccupiedNeighbours = this.neighbours.Count(w => w.Active);

                this.activeAfterUpdate = this.Active;

                if (this.Active)
                {
                    if (noOfOccupiedNeighbours < 2 || noOfOccupiedNeighbours > 3)
                    {
                        this.activeAfterUpdate = false;
                    }
                }
                else
                {
                    if (noOfOccupiedNeighbours == 3)
                    {
                        this.activeAfterUpdate = true;
                    }
                }

                return this.activeAfterUpdate;
            }

            public void Update()
            {
                this.Active = this.activeAfterUpdate;
            }

            public void EnsureNeighborsExist()
            {
                if (this.neighbours != null)
                    return;

                this.neighbours = new List<Cell>();

                for (int x = this.X - 1; x <= this.X + 1; x++)
                {
                    for (int y = this.Y - 1; y <= this.Y + 1; y++)
                    {
                        for (int z = this.Z - 1; z <= this.Z + 1; z++)
                        {
                            if (ignore4d)
                            {
                                AddKey(x, y, z, 0);
                            }
                            else
                            {
                                for (int h = this.H - 1; h <= this.H + 1; h++)
                                {
                                    AddKey(x, y, z, h);
                                }
                            }
                        }
                    }
                }

                void AddKey(int x, int y, int z, int h)
                {
                    var key = FormatCoordinates(x, y, z, h);

                    if (key != this.Coordinates)
                    {
                        this.neighbours.Add(grid.GetOrCreateInstance(key));
                    }
                }
            }
        }
    }
}
