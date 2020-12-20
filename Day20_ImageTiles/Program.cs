using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    class Program
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

            var cornerTiles = GetCornerTiles(tiles);

            long factorOfCornerTileIds = 1;
            foreach (var cornerTile in cornerTiles)
            {
                factorOfCornerTileIds *= cornerTile.Id;
            }

            Console.WriteLine($"Part 1: {factorOfCornerTileIds}");

            AssembleTilesToImage(tiles, cornerTiles);
        }

        private static IList<ImageTile> GetCornerTiles(IList<ImageTile> tiles)
        {
            var cornerTiles = new List<ImageTile>();

            foreach (var tile in tiles)
            {
                tile.Reset();

                int foundNeighbours = 0;

                foreach (var potentialN in tiles)
                {
                    if (tile == potentialN) continue;

                    if (ArePotentialNeighbours(tile, potentialN))
                    {
                        foundNeighbours++;
                    }
                }

                if (foundNeighbours < 1 || foundNeighbours > 4)
                    throw new Exception();

                if (foundNeighbours == 2)
                {
                    cornerTiles.Add(tile);
                }
            }

            return cornerTiles;
        }

        private static bool ArePotentialNeighbours(ImageTile tile1, ImageTile tile2)
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

        private static Dictionary<(int x, int y), ImageTile>? AssembleTilesToImage(IList<ImageTile> tiles, IList<ImageTile> cornerTiles)
        {
            int sideLength = (int)Math.Sqrt(tiles.Count) - 1;
            var topLeftPos = (0, 0);
            var topRightPos = (sideLength, 0);
            var bottomLeftPos = (0, sideLength);
            var bottomRightPos = (sideLength, sideLength);

            var arrangements = cornerTiles.GetAllOrdersOfList().ToList();

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

                var topRight = alignedTiles[1];
                posInGrid[topRight] = topRightPos;
                grid[topRightPos] = topRight;

                var bottomLeft = alignedTiles[2];
                posInGrid[bottomLeft] = bottomLeftPos;
                grid[bottomLeftPos] = bottomLeft;

                var bottomRight = alignedTiles[3];
                posInGrid[bottomRight] = bottomRightPos;
                grid[bottomRightPos] = bottomRight;

                for (int topLeftCornerArrangement = 0; topLeftCornerArrangement < 4; topLeftCornerArrangement++)
                {
                    topLeft.SetIntoConfig(topLeftCornerArrangement);

                    for (int topRightCornerArrangement = 0; topRightCornerArrangement < 4; topRightCornerArrangement++)
                    {
                        topRight.SetIntoConfig(topRightCornerArrangement);

                        for (int bottomLeftCornerArrangement = 0; bottomLeftCornerArrangement < 4; bottomLeftCornerArrangement++)
                        {
                            bottomLeft.SetIntoConfig(bottomLeftCornerArrangement);

                            for (int bottomRightCornerArrangement = 0; bottomRightCornerArrangement < 4; bottomRightCornerArrangement++)
                            {
                                bottomRight.SetIntoConfig(bottomRightCornerArrangement);

                                for (int topLeftCornerRotation = 0; topLeftCornerRotation < 4; topLeftCornerRotation++)
                                {
                                    topLeft.Rotate90();

                                    for (int topRightCornerRotation = 0; topRightCornerRotation < 4; topRightCornerRotation++)
                                    {
                                        topRight.Rotate90();

                                        for (int bottomLeftCornerRotation = 0; bottomLeftCornerRotation < 4; bottomLeftCornerRotation++)
                                        {
                                            bottomLeft.Rotate90();

                                            for (int bottomRightCornerRotation = 0; bottomRightCornerRotation < 4; bottomRightCornerRotation++)
                                            {
                                                bottomRight.Rotate90();

                                                var workingPosInGrid = posInGrid.ToDictionary(w => w.Key, w => w.Value);
                                                var workingGrid = grid.ToDictionary(w => w.Key, w => w.Value);
                                                var workingAlignedTiles = alignedTiles.ToList();
                                                var workingTilesToAlign = tilesToAlign.ToList();

                                                var nodes = new List<(int id, int x, int y, int config, int rotation)>();
                                                if (AttemptFillGrid(workingPosInGrid, workingGrid, workingAlignedTiles, workingTilesToAlign, nodes))
                                                {
                                                    return workingGrid;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static bool AttemptFillGrid(Dictionary<ImageTile, (int x, int y)> posInGrid, Dictionary<(int x, int y), ImageTile> grid, List<ImageTile> alignedTiles, List<ImageTile> tilesToAlign, IList<(int id, int x, int y, int config, int rotation)> alreadyVisited)
        {
            if (tilesToAlign.Count == 0)
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (!grid.ContainsKey((x, y)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            var placesToPut = new List<(ImageTile tile, int x, int y, int config, int rotation)>();

            for (int i = 0; i < tilesToAlign.Count; i++)
            {
                var tile = tilesToAlign[i];
                
                for (int config = 0; config < 4; config++)
                {
                    tile.SetIntoConfig(config);

                    for (int rotation = 0; rotation < 4; rotation++)
                    {
                        tile.Rotate90();

                        int x, y;

                        foreach (var at in alignedTiles)
                        {
                            if (tile.TopLine == at.BottomLine)
                            {
                                x = posInGrid[at].x;
                                y = posInGrid[at].y + 1;
                            }
                            else if (tile.BottomLine == at.TopLine)
                            {
                                x = posInGrid[at].x;
                                y = posInGrid[at].y - 1;
                            }
                            else if (tile.LeftLine == at.RightLine)
                            {
                                x = posInGrid[at].x + 1;
                                y = posInGrid[at].y;
                            }
                            else if (tile.RightLine == at.LeftLine)
                            {
                                x = posInGrid[at].x - 1;
                                y = posInGrid[at].y;
                            }
                            else continue;

                            // temp, just to check how much it would speed up
                            if (x < 0) continue;
                            if (y < 0) continue;
                            if (x >= 3) continue;
                            if (y >= 3) continue;

                            if (grid.ContainsKey((x, y)))
                            {
                                // think more what to do here
                                continue;
                            }

                            var leftPos = (x - 1, y);
                            if (grid.ContainsKey(leftPos))
                            {
                                var leftTile = grid[leftPos];
                                if (tile.LeftLine != leftTile.RightLine)
                                    continue;
                            }

                            var rightPos = (x + 1, y);
                            if (grid.ContainsKey(rightPos))
                            {
                                var rightTile = grid[rightPos];
                                if (tile.RightLine != rightTile.LeftLine)
                                    continue;
                            }

                            var belowPos = (x, y + 1);
                            if (grid.ContainsKey(belowPos))
                            {
                                var belowTile = grid[belowPos];
                                if (tile.BottomLine != belowTile.TopLine)
                                    continue;
                            }

                            var abovePos = (x, y - 1);
                            if (grid.ContainsKey(abovePos))
                            {
                                var aboveTile = grid[abovePos];
                                if (tile.TopLine != aboveTile.BottomLine)
                                    continue;
                            }

                            //we know we found match
                            placesToPut.Add((tile, x, y, config, rotation));
                        }
                    }
                }
            }

            foreach (var solution in placesToPut)
            {
                var comboId = (solution.tile.Id, solution.x, solution.y, solution.config, solution.rotation);

                if (alreadyVisited.Contains(comboId))
                    continue;

                alreadyVisited.Add(comboId);

                var workingPosInGrid = posInGrid.ToDictionary(w => w.Key, w => w.Value);
                var workingGrid = grid.ToDictionary(w => w.Key, w => w.Value);
                var workingAlignedTiles = alignedTiles.ToList();
                var workingTilesToAlign = tilesToAlign.ToList();

                workingPosInGrid[solution.tile] = (solution.x, solution.y);
                workingGrid[(solution.x, solution.y)] = solution.tile;
                workingAlignedTiles.Add(solution.tile);
                workingTilesToAlign.Remove(solution.tile);

                solution.tile.SetIntoConfig(solution.config);
                solution.tile.SetRotateSteps(solution.rotation);

                if (AttemptFillGrid(workingPosInGrid, workingGrid, workingAlignedTiles, workingTilesToAlign, alreadyVisited))
                {
                    posInGrid[solution.tile] = (solution.x, solution.y);
                    grid[(solution.x, solution.y)] = solution.tile;

                    return true;
                }
            }

            return false;
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

            public void Rotate90()
            {
                this.rotation++;
                this.ResetLines();
            }

            public void SetRotateSteps(int steps)
            {
                this.rotation = steps;
                this.ResetLines();
            }

            public void Reset()
            {
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

            public void FlipHorizontal()
            {
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

            public void FlipVertical()
            {
                this.rows.Reverse();
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
