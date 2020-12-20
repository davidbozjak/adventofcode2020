using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    class Program
    {
        static int maxPlaced = 0;

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

            AssembleTilesToImage(tiles, cornerTiles, neighbourhood);
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

        private static Dictionary<(int x, int y), ImageTile>? AssembleTilesToImage(IList<ImageTile> tiles, IList<ImageTile> cornerTiles, Dictionary<ImageTile, IList<ImageTile>> neighbourhood)
        {
            int sideLength = (int)Math.Sqrt(tiles.Count) - 1;
            var topLeftPos = (0, 0);
            //var topRightPos = (sideLength, 0);
            //var bottomLeftPos = (0, sideLength);
            //var bottomRightPos = (sideLength, sideLength);

            //var arrangements = cornerTiles.GetAllOrdersOfList().ToList();

            // Temp get just the right ID to confirm it works
            List<List<ImageTile>> arrangements = new List<List<ImageTile>>
            {
                new List<ImageTile>()
                {
                    tiles.First(w => w.Id == 1951),
                    tiles.First(w => w.Id == 3079),
                    tiles.First(w => w.Id == 2971),
                    tiles.First(w => w.Id == 1171)
                }
            };

            for (int i = 0; i < arrangements.Count; i++)
            {
                Dictionary<ImageTile, (int x, int y)> posInGrid = new Dictionary<ImageTile, (int x, int y)>();
                Dictionary<(int x, int y), ImageTile> grid = new Dictionary<(int x, int y), ImageTile>();

                var alignedTiles = arrangements[i].ToList();
                var tilesToAlign = tiles.Except(alignedTiles).ToList();

                Console.WriteLine(string.Join(" ", alignedTiles.Select(w => w.Id)));

                var topLeft = alignedTiles[0];
                posInGrid[topLeft] = topLeftPos;
                grid[topLeftPos] = topLeft;

                for (int config = 0; config < 4; config++)
                {
                    topLeft.UnFreeze();
                    topLeft.SetIntoConfig(config);
                    topLeft.Freeze();

                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        topLeft.UnFreeze();
                        topLeft.SetRotateSteps(rotation);
                        topLeft.Freeze();

                        var workingPosInGrid = posInGrid.ToDictionary(w => w.Key, w => w.Value);
                        var workingGrid = grid.ToDictionary(w => w.Key, w => w.Value);

                        //var placesToPut = new HashSet<(ImageTile tile, int x, int y, int config, int rotation)>();
                        //var tile = tiles.First(w => w.Id == 2311);
                        //int additions = FindOrientationsForTile(tile, posInGrid, grid, placesToPut, new[] { (0, 1) });

                        var nodes = new List<(int id, int x, int y, int config, int rotation)>();
                        if (AttemptFillGrid(workingPosInGrid, workingGrid, neighbourhood, nodes))
                        {
                            return workingGrid;
                        }
                    }
                }
            }

            return null;
        }

        private static bool AttemptFillGrid(Dictionary<ImageTile, (int x, int y)> posInGrid, Dictionary<(int x, int y), ImageTile> grid/*, List<ImageTile> alignedTiles, List<ImageTile> tilesToAlign*/, Dictionary<ImageTile, IList<ImageTile>> neighbourhood, IList<(int id, int x, int y, int config, int rotation)> alreadyVisited)
        {
            var placesToPut = new HashSet<(ImageTile tile, int x, int y, int config, int rotation)>();

            var tilesToPlace = posInGrid.Keys
                .SelectMany(w => neighbourhood[w])
                .ToHashSet()
                .Where(w => !posInGrid.ContainsKey(w))
                .ToList();

            bool IsValidLocation((int, int) value)
            {
                (int x, int y) = value;
                if (x < 0 || x > 3) return false;
                if (y < 0 || y > 3) return false;

                return true;
            }

            IEnumerable<(int x, int y)> GetNeighbouringLocations((int, int) value)
            {
                (int x, int y) = value;

                yield return (x - 1, y);
                yield return (x + 1, y);
                yield return (x, y - 1);
                yield return (x, y + 1);
            }

            if (tilesToPlace.Count == 0)
                return true;

            for (int i = 0; i < tilesToPlace.Count; i++)
            {   
                var tile = tilesToPlace[i];

                var gapsToFill = neighbourhood[tile]
                    .Where(w => posInGrid.ContainsKey(w))
                    .Select(w => posInGrid[w])
                    .SelectMany(w => GetNeighbouringLocations(w))
                    .Where(IsValidLocation)
                    .ToHashSet()
                    .ToList();

                int additions = FindOrientationsForTile(tile, posInGrid, grid, placesToPut, gapsToFill);

                //if (additions == 0)
                //{
                //    // we were unable to place a neighour. This is not the right branch.
                //    return false;
                //}
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
                solution.tile.Freeze();

                if (AttemptFillGrid(workingPosInGrid, workingGrid, neighbourhood, alreadyVisited))
                {
                    posInGrid[solution.tile] = (solution.x, solution.y);
                    grid[(solution.x, solution.y)] = solution.tile;

                    return true;
                }
                else
                {
                    solution.tile.UnFreeze();
                }
            }

            return false;
        }

        private static int FindOrientationsForTile(ImageTile tile, Dictionary<ImageTile, (int x, int y)> posInGrid, Dictionary<(int x, int y), ImageTile> grid, HashSet<(ImageTile tile, int x, int y, int config, int rotation)> placesToPut, IList<(int x, int y)> tilesToFill)
        {
            int addition = 0;

            var target = posInGrid.Keys.First();

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

        class ImageTile
        {
            public int Id { get; private set;  }

            public long TopLine => this.cachedTopLine.Value;
            public long BottomLine => this.cachedBottomLine.Value;
            public long LeftLine => this.cachedLeftLine.Value;
            public long RightLine => this.cachedRightLine.Value;

            private readonly List<string> rows = new List<string>();

            private readonly Cached<long> cachedLeftLine;
            private readonly Cached<long> cachedRightLine;
            private readonly Cached<long> cachedBottomLine;
            private readonly Cached<long> cachedTopLine;

            private int rotation = 0;
            private bool isFlippedHorizontally = false;
            private bool isFlippedVerticallly = false;
            private bool isFrozen = false;

            public ImageTile()
            {
                this.cachedLeftLine = new Cached<long>(() => this.CalculateLine(Edge.Left));
                this.cachedRightLine = new Cached<long>(() => this.CalculateLine(Edge.Right));
                this.cachedBottomLine = new Cached<long>(() => this.CalculateLine(Edge.Bottom));
                this.cachedTopLine = new Cached<long>(() => this.CalculateLine(Edge.Top));
            }

            public void AddRow(string row)
            {
                if (row.StartsWith("Tile"))
                {
                    this.SetId(row);
                }
                else
                {
                    this.rows.Add(row);
                    this.ResetLines();
                }
            }

            public void Freeze()
            {
                this.isFrozen = true;
            }

            public void UnFreeze()
            {
                this.isFrozen = false;
            }

            public void Rotate90()
            {
                if (this.isFrozen) throw new Exception();

                this.rotation++;
                this.ResetLines();
            }

            public void SetRotateSteps(int steps)
            {
                if (this.isFrozen) throw new Exception();

                this.rotation = steps;
                this.ResetLines();
            }

            public int Rotation => this.rotation;

            public void Reset()
            {
                if (this.isFrozen) throw new Exception();

                if (this.isFlippedHorizontally)
                {
                    this.FlipHorizontal();
                }

                if (this.isFlippedVerticallly)
                {
                    this.FlipVertical();
                }

                this.rotation = 0;
                this.ResetLines();
            }

            public void SetIntoConfig(int config)
            {
                if (this.isFrozen) throw new Exception();
                this.Reset();

                if (config == 1)
                {
                    this.FlipVertical();
                }
                else if (config == 2)
                {
                    this.FlipHorizontal();
                }
                else if (config == 3)
                {
                    this.FlipVertical();
                    this.FlipHorizontal();
                }
            }

            public override string ToString() => this.Id.ToString();

            private void FlipHorizontal()
            {
                if (this.isFrozen) throw new Exception();

                var flippedRows = new List<string>();

                for (int i = 0; i < this.rows.Count; i++)
                {
                    flippedRows.Add(new string(this.rows[i].Reverse().ToArray()));
                }

                this.rows.Clear();
                this.rows.AddRange(flippedRows);

                this.ResetLines();
                this.isFlippedHorizontally = !this.isFlippedHorizontally;
            }

            private void FlipVertical()
            {
                this.rows.Reverse();
                this.ResetLines();
                this.isFlippedVerticallly = !this.isFlippedVerticallly;
            }

            private void SetId(string row)
            {
                var idString = row[(row.IndexOf(" ") + 1)..^1];
                this.Id = int.Parse(idString);
            }

            enum Edge { Left = 1, Right = 2, Bottom = 3, Top = 4 }
            private long CalculateLine(Edge edge)
            {
                string edgeStr = GetEdgeString(edge);
                long value = 0;

                for (int i = 0; i < edgeStr.Length; i++)
                {
                    if (edgeStr[i] == '#')
                    {
                        value += (long)Math.Pow(2, i);
                    }
                }

                return value;
            }

            private string GetEdgeString(Edge edge)
            {
                var edgeAfterTotation = this.rotation + edge;
                while ((int)edgeAfterTotation > 4) edgeAfterTotation -= 4;
                while ((int)edgeAfterTotation <= 0) edgeAfterTotation += 4;

                if (edgeAfterTotation == Edge.Top)
                {
                    return this.rows[0];
                }
                else if (edgeAfterTotation == Edge.Bottom)
                {
                    return this.rows.Last();
                }
                else if (edgeAfterTotation == Edge.Left)
                {
                    return new string(this.rows.Select(w => w[0]).ToArray());
                }
                else if (edgeAfterTotation == Edge.Right)
                {
                    return new string(this.rows.Select(w => w[^1]).ToArray());
                }
                else throw new Exception();
            }

            private void ResetLines()
            {
                this.cachedLeftLine.Reset();
                this.cachedRightLine.Reset();
                this.cachedBottomLine.Reset();
                this.cachedTopLine.Reset();
            }
        }
    }
}
