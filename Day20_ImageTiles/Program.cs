using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var parser = new MultiLineParser<ImageTile>(() => new ImageTile(), (tile, row) => tile.AddRow(row));
            using var inputProvider = new InputProvider<ImageTile?>("Input.txt", parser.AddLine)
            {
                EndAtEmptyLine = false
            };

            var inputs = inputProvider.ToList();
            var tiles = inputs.Where(w => w != null).Cast<ImageTile>().ToList();

            var neighbourhood = FindNeighbouringTiles(tiles);

            var cornerTiles = neighbourhood.Where(w => w.Value.Count == 2).Select(w => w.Key).ToList();

            long factorOfCornerTileIds = 1;
            foreach (var cornerTile in cornerTiles)
            {
                factorOfCornerTileIds *= cornerTile.Id;
            }

            Console.WriteLine($"Part 1: {factorOfCornerTileIds}");

            var grid = AssembleTilesToImage(cornerTiles[0], tiles, neighbourhood);

            if (grid == null) throw new Exception();

            var sideLength = (int)Math.Sqrt(tiles.Count);

            var fullImage = new Image(sideLength, cornerTiles[0]);
            for (int x = 0; x < sideLength; x++)
            {
                for (int y = 0; y < sideLength; y++)
                {
                    fullImage.Insert(grid[(x, y)], x, y);
                }
            }

            var seaMonster = new ImageMask();
            seaMonster.AddRow("                  # ");
            seaMonster.AddRow("#    ##    ##    ###");
            seaMonster.AddRow(" #  #  #  #  #  #   ");

            List<(int x, int y)>? monstersFound = null;
            for (int config = 0; config < 4; config++)
            {
                fullImage.SetIntoConfig(config);

                for (int rotation = 0; rotation < 4; rotation++)
                {
                    fullImage.SetRotateSteps(rotation);

                    var monsters = fullImage.FindMaskWithinImage(seaMonster);
                    if (monsters.Count > (monstersFound?.Count ?? 0))
                    {
                        monstersFound = monsters;
                    }
                }
            }

            if (monstersFound == null) throw new Exception();

            int total = fullImage.Count(w => w == '#');
            int minusSeaMonster = total - monstersFound.Count * seaMonster.MaskedLocationsCount;
            Console.WriteLine($"Part 2: Found {monstersFound.Count} monsters, marked tiles without monsters: {minusSeaMonster}");

            Console.WriteLine();
            fullImage.PrintImageWithinImage(seaMonster, monstersFound, Console.WriteLine);
        }

        private static Dictionary<ImageTile, IList<ImageTile>> FindNeighbouringTiles(IList<ImageTile> tiles)
        {
            var neighbourhood = new Dictionary<ImageTile, IList<ImageTile>>();

            foreach (var tile in tiles)
            {
                tile.Reset();
                var neighbours = new List<ImageTile>();

                foreach (var potentialN in tiles)
                {
                    if (tile == potentialN) continue;

                    if (ArePotentialNeighbours(tile, potentialN))
                    {
                        neighbours.Add(potentialN);
                    }
                }

                if (neighbours.Count < 1 || neighbours.Count > 4)
                    throw new Exception();

                neighbourhood.Add(tile, neighbours);
            }

            return neighbourhood;

            static bool ArePotentialNeighbours(ImageTile tile1, ImageTile tile2)
            {
                for (int config = 0; config < 4; config++)
                {
                    tile2.SetIntoConfig(config);

                    for (int i = 0; i < 4; i++)
                    {
                        if (tile1.TopLine == tile2.BottomLine ||
                            tile1.BottomLine == tile2.TopLine ||
                            tile1.LeftLine == tile2.RightLine ||
                            tile1.RightLine == tile2.LeftLine)
                        {
                            return true;
                        }

                        tile2.Rotate90();
                    }
                }

                return false;
            }
        }

        private static Dictionary<(int x, int y), ImageTile>? AssembleTilesToImage(ImageTile imageTile, IList<ImageTile> tiles, Dictionary<ImageTile, IList<ImageTile>> neighbourhood)
        {
            Dictionary<ImageTile, (int x, int y)> posInGrid = new Dictionary<ImageTile, (int x, int y)>();
            Dictionary<(int x, int y), ImageTile> grid = new Dictionary<(int x, int y), ImageTile>();

            var topLeftPos = (0, 0);
            var topLeft = imageTile;
            var sideLength = (int)Math.Sqrt(tiles.Count);
            posInGrid[topLeft] = topLeftPos;
            grid[topLeftPos] = topLeft;

            for (int config = 0; config < 4; config++)
            {
                topLeft.SetIntoConfig(config);

                for (int rotation = 0; rotation < 4; rotation++)
                {
                    topLeft.SetRotateSteps(rotation);

                    var workingPosInGrid = posInGrid.ToDictionary(w => w.Key, w => w.Value);
                    var workingGrid = grid.ToDictionary(w => w.Key, w => w.Value);

                    var nodes = new List<(int id, int x, int y, int config, int rotation)>();
                    if (AttemptFillGrid(workingPosInGrid, workingGrid, neighbourhood, nodes, IsValidLocation))
                    {
                        return workingGrid;
                    }
                }
            }

            return null;

            bool IsValidLocation((int, int) value)
            {
                (int x, int y) = value;
                if (x < 0 || x > sideLength - 1) return false;
                if (y < 0 || y > sideLength - 1) return false;

                return true;
            }
        }

        private static bool AttemptFillGrid(Dictionary<ImageTile, (int x, int y)> posInGrid, Dictionary<(int x, int y), ImageTile> grid, Dictionary<ImageTile, IList<ImageTile>> neighbourhood, IList<(int id, int x, int y, int config, int rotation)> alreadyVisited, Func<(int, int), bool> locationValidator)
        {
            var placesToPut = new HashSet<(ImageTile tile, int x, int y, int config, int rotation)>();

            var tilesToPlace = posInGrid.Keys
                .SelectMany(w => neighbourhood[w])
                .ToHashSet()
                .Where(w => !posInGrid.ContainsKey(w))
                .ToList();

            if (tilesToPlace.Count == 0)
                return true;

            for (int i = 0; i < tilesToPlace.Count; i++)
            {   
                var tile = tilesToPlace[i];

                var gapsToFill = neighbourhood[tile]
                    .Where(w => posInGrid.ContainsKey(w))
                    .Select(w => posInGrid[w])
                    .SelectMany(w => GetNeighbouringLocations(w))
                    .Where(locationValidator)
                    .ToHashSet()
                    .ToList();

                int additions = FindOrientationsForTile(tile, grid, placesToPut, gapsToFill);
            }

            foreach (var solution in placesToPut)
            {
                var comboId = (solution.tile.Id, solution.x, solution.y, solution.config, solution.rotation);

                if (alreadyVisited.Contains(comboId))
                    continue;

                alreadyVisited.Add(comboId);

                var workingPosInGrid = posInGrid.ToDictionary(w => w.Key, w => w.Value);
                var workingGrid = grid.ToDictionary(w => w.Key, w => w.Value);

                workingPosInGrid[solution.tile] = (solution.x, solution.y);
                workingGrid[(solution.x, solution.y)] = solution.tile;

                solution.tile.SetIntoConfig(solution.config);
                solution.tile.SetRotateSteps(solution.rotation);

                if (AttemptFillGrid(workingPosInGrid, workingGrid, neighbourhood, alreadyVisited, locationValidator))
                {
                    foreach (var key in workingPosInGrid.Keys)
                    {
                        if (!posInGrid.ContainsKey(key))
                            posInGrid.Add(key, workingPosInGrid[key]);
                    }

                    foreach (var key in workingGrid.Keys)
                    {
                        if (!grid.ContainsKey(key))
                            grid.Add(key, workingGrid[key]);
                    }

                    return true;
                }
            }

            return false;

            static IEnumerable<(int x, int y)> GetNeighbouringLocations((int, int) value)
            {
                (int x, int y) = value;

                yield return (x - 1, y);
                yield return (x + 1, y);
                yield return (x, y - 1);
                yield return (x, y + 1);
            }
        }

        private static int FindOrientationsForTile(ImageTile tile, Dictionary<(int x, int y), ImageTile> grid, HashSet<(ImageTile tile, int x, int y, int config, int rotation)> placesToPut, IList<(int x, int y)> tilesToFill)
        {
            int addition = 0;

            foreach ((int x, int y) in tilesToFill)
            {
                var leftPos = (x - 1, y);
                var rightPos = (x + 1, y);
                var abovePos = (x, y - 1);
                var belowPos = (x, y + 1);

                for (int config = 0; config < 4; config++)
                {
                    tile.SetIntoConfig(config);

                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        tile.SetRotateSteps(rotation);

                        if (grid.ContainsKey(leftPos))
                        {
                            var leftTile = grid[leftPos];
                            if (tile.LeftLine != leftTile.RightLine)
                                continue;
                        }

                        if (grid.ContainsKey(rightPos))
                        {
                            var rightTile = grid[rightPos];
                            if (tile.RightLine != rightTile.LeftLine)
                                continue;
                        }

                        if (grid.ContainsKey(belowPos))
                        {
                            var belowTile = grid[belowPos];
                            if (tile.BottomLine != belowTile.TopLine)
                                continue;
                        }

                        if (grid.ContainsKey(abovePos))
                        {
                            var aboveTile = grid[abovePos];
                            if (tile.TopLine != aboveTile.BottomLine)
                                continue;
                        }

                        addition++;
                        placesToPut.Add((tile, x, y, config, rotation));
                    }
                }
            }

            return addition;
        }
    }
}
