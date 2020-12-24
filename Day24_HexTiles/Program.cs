using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Day24_HexTiles
{
    class Program
    {
        static readonly UniqueFactory<(int x, int y), Tile> tileFactory = new UniqueFactory<(int x, int y), Tile>(pos => new Tile(pos.x, pos.y));

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<Tile?>("Input.txt", GetTile);

            var input = inputProvider.Where(w => w != null).Cast<Tile>().ToList();

            Part1(input);

            Part2();
        }

        static void Part1(IList<Tile> input)
        {
            foreach (var tile in input)
            {
                tile.Flip();
            }

            Console.WriteLine($"Part 1: Number of black tiles: {input.Count(w => !w.Color)}");
        }

        static void Part2()
        {
            for (int day = 0; day < 100; day++)
            {
                var tilesToFlip = new List<Tile>();

                // insure neighbours exist
                foreach (var tile in tileFactory.AllCreatedInstances)
                {
                    var neighbors = new[]
                    {
                        tileFactory.GetOrCreateInstance((tile.X + 10, tile.Y)),
                        tileFactory.GetOrCreateInstance((tile.X + 5, tile.Y + 5)),
                        tileFactory.GetOrCreateInstance((tile.X - 5, tile.Y + 5)),
                        tileFactory.GetOrCreateInstance((tile.X - 10, tile.Y)),
                        tileFactory.GetOrCreateInstance((tile.X - 5, tile.Y - 5)),
                        tileFactory.GetOrCreateInstance((tile.X - 5, tile.Y - 5))
                    };
                }

                var tilesToCheck = tileFactory.AllCreatedInstances.ToList();

                foreach (var tile in tilesToCheck)
                {
                    var neighbors = new[]
                    {
                        tileFactory.GetOrCreateInstance((tile.X + 10, tile.Y)),
                        tileFactory.GetOrCreateInstance((tile.X + 5, tile.Y + 5)),
                        tileFactory.GetOrCreateInstance((tile.X - 5, tile.Y + 5)),
                        tileFactory.GetOrCreateInstance((tile.X - 10, tile.Y)),
                        tileFactory.GetOrCreateInstance((tile.X - 5, tile.Y - 5)),
                        tileFactory.GetOrCreateInstance((tile.X + 5, tile.Y - 5))
                    };

                    var blackNeighbours = neighbors.Count(w => !w.Color);

                    if (tile.Color)
                    {
                        if (blackNeighbours == 2)
                        {
                            tilesToFlip.Add(tile);
                        }
                    }
                    else
                    {
                        if (blackNeighbours == 0 || blackNeighbours > 2)
                        {
                            tilesToFlip.Add(tile);
                        }
                    }
                }

                foreach (var tile in tilesToFlip)
                {
                    tile.Flip();
                }
            }

            Console.WriteLine($"Part 2: Black tiles after 100 days: {tileFactory.AllCreatedInstances.Count(w => !w.Color)}");
        }

        static bool GetTile(string? input, out Tile? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(input)) return false;

            try
            {
                var pos = ParseCoordinates(input);

                value = tileFactory.GetOrCreateInstance(pos);

                return true;
            }
            catch
            {
                return false;
            }
        }

        static (int x, int y) ParseCoordinates(string path)
        {
            int x = 0, y = 0;

            for (int i = 0; i < path.Length; i++)
            {
                if (path.Length >= i + 2)
                {
                    var twoLetterInstruction = path[i..(i + 2)];

                    if (twoLetterInstruction == "se")
                    {
                        x += 5;
                        y += 5;

                        i++;
                        continue;
                    }
                    else if (twoLetterInstruction == "sw")
                    {
                        x -= 5;
                        y += 5;

                        i++;
                        continue;
                    }
                    else if (twoLetterInstruction == "nw")
                    {
                        x -= 5;
                        y -= 5;

                        i++;
                        continue;
                    }
                    else if (twoLetterInstruction == "ne")
                    {
                        x += 5;
                        y -= 5;

                        i++;
                        continue;
                    }
                }

                switch (path[i])
                {
                    case 'e':
                        x += 10;
                        break;
                    case 'w':
                        x -= 10;
                        break;
                    case 's':
                        y += 10;
                        break;
                    case 'n':
                        y -= 10;
                        break;
                    default: throw new Exception();
                }
            }

            return (x, y);
        }

        class Tile
        {
            public int X { get; }

            public int Y { get; }

            public bool Color { get; private set; }

            public Tile (int x, int y)
            {
                this.X = x;
                this.Y = y;
                this.Color = true;
            }

            public void Flip()
            {
                this.Color = !this.Color;
            }

            public override string ToString()
            {
                return $"x:{this.X} y:{this.Y}";
            }
        }
    }
}
