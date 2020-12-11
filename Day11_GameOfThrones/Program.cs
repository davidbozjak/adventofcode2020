using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Day11_GameOfThrones
{
    class Program
    {
        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var map = new MapBuilder();
            inputProvider.ToList().ForEach(w => map.AddRow(w));

            foreach (var cell in map.Cells)
            {
                cell.SetNeiboughurs(map.Cells);
                cell.SetPotentiallyVisible(map.Cells);
            }

            Part1(map);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach (var cell in map.Cells)
            {
                cell.Reset();
            }

            Part2(map);
        }

        private static void Part1(MapBuilder map)
        {
            int totalOccupied = 0;
            int newTotalOccupied = 0;

            do
            {
                totalOccupied = newTotalOccupied;

                newTotalOccupied = map.Cells
                    .Where(w => !w.IsFloor)
                    .Count(w => w.EvaluateAdjecent());

                Console.WriteLine($"Part1: No of occupied: {newTotalOccupied}");

                foreach (var cell in map.Cells)
                    cell.Update();

            } while (totalOccupied != newTotalOccupied);
        }

        private static void Part2(MapBuilder map)
        {
            int totalOccupied = 0;
            int newTotalOccupied = 0;

            do
            {
                totalOccupied = newTotalOccupied;

                newTotalOccupied = map.Cells
                    .Where(w => !w.IsFloor)
                    .Count(w => w.EvaluateVisible());

                Console.WriteLine($"Part2: No of occupied: {newTotalOccupied}");

                foreach (var cell in map.Cells)
                    cell.Update();

            } while (totalOccupied != newTotalOccupied);
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
            private readonly List<Cell> firstVisible = new List<Cell>();

            public Cell (int x, int y, bool isFloor)
            {
                this.X = x;
                this.Y = y;
                this.IsFloor = isFloor;
                this.IsOccupied = false;
            }

            public void SetNeiboughurs(IEnumerable<Cell> allCells)
            {
                this.neighbours.Clear();
                this.neighbours.AddRange(allCells.Where(w => !w.IsFloor && this.IsNeighbour(w)));
            }

            public void SetPotentiallyVisible(IEnumerable<Cell> allCells)
            {
                this.firstVisible.Clear();

                int maxX = allCells.Select(w => w.X).Max();
                int maxY = allCells.Select(w => w.Y).Max();

                allCells = allCells.Where(w => !w.IsFloor);

                // left
                AddInDirection(this.X - 1, this.Y, -1, 0, x => x >= 0, y => true);

                // right
                AddInDirection(this.X + 1, this.Y, 1, 0, x => x <= maxX, y => true);

                // up
                AddInDirection(this.X, this.Y - 1, 0, -1, x => true, y => y >= 0);

                // down
                AddInDirection(this.X, this.Y + 1, 0, 1, x => true, y => y <= maxY);

                // left down
                AddInDirection(this.X - 1, this.Y + 1, -1, 1, x => x >= 0, y => y <= maxY);

                // left up
                AddInDirection(this.X - 1, this.Y - 1, -1, -1, x => x >= 0, y => y >= 0);

                // right up
                AddInDirection(this.X + 1, this.Y - 1, 1, -1, x => x <= maxX, y => y >= 0);

                // right down
                AddInDirection(this.X + 1, this.Y + 1, 1, 1, x => x <= maxX, y => y <= maxY);

                void AddInDirection(int initialX, int initialY, int stepX, int stepY, Func<int, bool> checkX, Func<int, bool> checkY)
                {
                    for (int x = initialX, y = initialY; checkX(x) && checkY(y); y += stepY, x += stepX)
                    {
                        var seat = allCells.FirstOrDefault(w => w.X == x && w.Y == y);

                        if (seat != null)
                        {
                            this.firstVisible.Add(seat);
                            break;
                        }
                    }
                }
            }

            public bool EvaluateAdjecent()
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
                    if (noOfOccupiedNeighbours >= 4)
                    {
                        this.occupiedAfterUpdate = false;
                    }
                }

                return this.occupiedAfterUpdate;
            }

            public bool EvaluateVisible()
            {
                int noOfOccupiedNeighbours = this.firstVisible.Count(w => w.IsOccupied);

                if (!this.IsOccupied)
                {
                    if (noOfOccupiedNeighbours == 0)
                    {
                        this.occupiedAfterUpdate = true;
                    }
                }
                else
                {
                    if (noOfOccupiedNeighbours >= 5)
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

            public void Reset()
            {
                this.IsOccupied = false;
                this.occupiedAfterUpdate = false;
            }

            public bool IsNeighbour(Cell potentialNeighbour)
            {
                if (Math.Abs(this.X - potentialNeighbour.X) > 1) return false;
                if (Math.Abs(this.Y - potentialNeighbour.Y) > 1) return false;
                if (this.X == potentialNeighbour.X && this.Y == potentialNeighbour.Y) return false;

                return true;
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

        }
    }
}
