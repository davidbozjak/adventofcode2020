using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11_GameOfThrones
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);
            var inputs = inputProvider.ToList();

            var map = new MapBuilder();
            inputs.ForEach(w => map.AddRow(w));
            map.SetNeighbours();

            var part1 = RunTillStable(map, 4);

            Console.WriteLine($"Part 1: Stabilizes at {part1}");

            map = new MapBuilder();
            inputs.ForEach(w => map.AddRow(w));
            map.SetFirstVisible();

            var part2 = RunTillStable(map, 5);

            Console.WriteLine($"Part 2: Stabilizes at {part2}");
        }

        private static int RunTillStable(MapBuilder map, int maxTolerableNeighbours)
        {
            int totalOccupied = 0;
            int newTotalOccupied = 0;

            do
            {
                totalOccupied = newTotalOccupied;

                newTotalOccupied = map.Cells
                    .Where(w => !w.IsFloor)
                    .Count(w => w.Evaluate(maxTolerableNeighbours));

                foreach (var cell in map.Cells)
                    cell.Update();

            } while (totalOccupied != newTotalOccupied);

            return totalOccupied;
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

            public bool IsFloor { get; }

            public bool IsOccupied { get; private set; }
            private bool occupiedAfterUpdate;

            private readonly List<Cell> neighbours = new List<Cell>();

            public Cell (int x, int y, bool isFloor)
            {
                this.X = x;
                this.Y = y;
                this.IsFloor = isFloor;
                this.IsOccupied = false;
            }

            public void SetNeiboughurs(IEnumerable<Cell> neighbours)
            {
                this.neighbours.Clear();
                this.neighbours.AddRange(neighbours);
            }

            public bool Evaluate(int maxTolerableNeighbours)
            {
                int noOfOccupiedNeighbours = this.neighbours.Count(w => w.IsOccupied);

                if (!this.IsOccupied)
                {
                    if (noOfOccupiedNeighbours == 0)
                    {
                        this.occupiedAfterUpdate = true;
                    }
                }
                else
                {
                    if (noOfOccupiedNeighbours >= maxTolerableNeighbours)
                    {
                        this.occupiedAfterUpdate = false;
                    }
                }

                return this.occupiedAfterUpdate;
            }

            public void Update()
            {
                this.IsOccupied = this.occupiedAfterUpdate;
            }
        }

        class MapBuilder
        {
            private readonly List<Cell> cells = new List<Cell>();
            private int rows = 0;

            public IReadOnlyList<Cell> Cells => this.cells.AsReadOnly();

            public void AddRow(string row)
            {
                if (string.IsNullOrWhiteSpace(row)) return;

                for (int x = 0; x < row.Length; x++)
                {
                    bool isFloor = row[x] == '.';
                    cells.Add(new Cell(x, rows, isFloor));
                }

                rows++;
            }

            public void SetNeighbours()
            {
                var seats = this.Cells.Where(w => !w.IsFloor);

                foreach (var cell in seats)
                {
                    cell.SetNeiboughurs(seats.Where(w => IsNeighbour(cell, w)));
                }

                static bool IsNeighbour(Cell c, Cell potentialNeighbour)
                {
                    if (Math.Abs(c.X - potentialNeighbour.X) > 1) return false;
                    if (Math.Abs(c.Y - potentialNeighbour.Y) > 1) return false;
                    if (c.X == potentialNeighbour.X && c.Y == potentialNeighbour.Y) return false;

                    return true;
                }
            }

            public void SetFirstVisible()
            {
                var seats = this.Cells.Where(w => !w.IsFloor);

                int maxX = seats.Select(w => w.X).Max();
                int maxY = seats.Select(w => w.Y).Max();

                foreach (var cell in seats)
                {
                    var neighbours = new List<Cell>();

                    // left
                    AddInDirection(neighbours, cell.X - 1, cell.Y, -1, 0, x => x >= 0, y => true);

                    // right
                    AddInDirection(neighbours, cell.X + 1, cell.Y, 1, 0, x => x <= maxX, y => true);

                    // up
                    AddInDirection(neighbours, cell.X, cell.Y - 1, 0, -1, x => true, y => y >= 0);

                    // down
                    AddInDirection(neighbours, cell.X, cell.Y + 1, 0, 1, x => true, y => y <= maxY);

                    // left down
                    AddInDirection(neighbours, cell.X - 1, cell.Y + 1, -1, 1, x => x >= 0, y => y <= maxY);

                    // left up
                    AddInDirection(neighbours, cell.X - 1, cell.Y - 1, -1, -1, x => x >= 0, y => y >= 0);

                    // right up
                    AddInDirection(neighbours, cell.X + 1, cell.Y - 1, 1, -1, x => x <= maxX, y => y >= 0);

                    // right down
                    AddInDirection(neighbours, cell.X + 1, cell.Y + 1, 1, 1, x => x <= maxX, y => y <= maxY);

                    cell.SetNeiboughurs(neighbours);
                }

                void AddInDirection(IList<Cell> neighbours, int initialX, int initialY, int stepX, int stepY, Func<int, bool> checkX, Func<int, bool> checkY)
                {
                    for (int x = initialX, y = initialY; checkX(x) && checkY(y); y += stepY, x += stepX)
                    {
                        var seat = seats.FirstOrDefault(w => w.X == x && w.Y == y);

                        if (seat != null)
                        {
                            neighbours.Add(seat);
                            break;
                        }
                    }
                }
            }
        }
    }
}
