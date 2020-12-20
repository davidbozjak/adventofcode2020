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

            //AssembleTilesToImage(tiles);
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

                    potentialN.Reset();
                    bool couldBeN = false;

                    for (int config = 0; !couldBeN && config < 4; config++)
                    {
                        potentialN.SetIntoConfig(config);

                        for (int i = 0; i < 4; i++)
                        {
                            if (tile.TopLine == potentialN.BottomLine ||
                                tile.BottomLine == potentialN.TopLine ||
                                tile.LeftLine == potentialN.RightLine ||
                                tile.RightLine == potentialN.LeftLine)
                            {
                                couldBeN = true;
                                foundNeighbours++;
                                break;
                            }


                            potentialN.Rotate90();
                        }
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

        private static long AssembleTilesToImage(IList<ImageTile> tiles)
        {
            int possibleTileArrangements = 50;
            for (int startTileTransformCount = 1; startTileTransformCount < possibleTileArrangements; startTileTransformCount++)
            {
                var startTile = tiles[0];

                if (startTileTransformCount % 4 == 0 || startTileTransformCount % 6 == 0)
                {
                    startTile.FlipHorizontal();
                }

                startTile.Rotate90();

                //Dictionary<ImageTile, ImageTile> leftOf = new Dictionary<ImageTile, ImageTile>();
                //Dictionary<ImageTile, ImageTile> rightOf = new Dictionary<ImageTile, ImageTile>();
                //Dictionary<ImageTile, ImageTile> above = new Dictionary<ImageTile, ImageTile>();
                //Dictionary<ImageTile, ImageTile> below = new Dictionary<ImageTile, ImageTile>();
                //Dictionary<ImageTile, int> xPos = new Dictionary<ImageTile, int>();
                //Dictionary<ImageTile, int> yPos = new Dictionary<ImageTile, int>();
                Dictionary<ImageTile, (int x, int y)> posInGrid = new Dictionary<ImageTile, (int x, int y)>();
                Dictionary<(int x, int y), ImageTile> grid = new Dictionary<(int x, int y), ImageTile>();

                var tilesToAlign = tiles.Skip(1).ToList();
                var alignedTiles = new List<ImageTile>
                {
                    startTile
                };
                posInGrid[alignedTiles[0]] = (0, 0);
                grid[(0, 0)] = alignedTiles[0];

                //xPos[alignedTiles[0]] = 0;
                //yPos[alignedTiles[0]] = 0;

                bool foundAnyMatch = true;
                while (foundAnyMatch && tilesToAlign.Count > 0)
                {
                    bool foundMatch = false;
                    foundAnyMatch = false;
                    for (int i = 0; !foundMatch && i < tilesToAlign.Count; i++)
                    {
                        var tile = tilesToAlign[i];

                        for (int transformCount = 1; !foundMatch && transformCount < possibleTileArrangements; transformCount++)
                        {
                            if (transformCount % 4 == 0 || transformCount % 6 == 0)
                            {
                                tile.FlipHorizontal();
                            }

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
                                foundMatch = true;
                                foundAnyMatch = true;

                                alignedTiles.Add(tile);
                                tilesToAlign.Remove(tile);

                                posInGrid[tile] = (x, y);
                                grid[(x, y)] = tile;

                                break;
                            }
                        }
                    }
                }

                if (tilesToAlign.Count == 0)
                {
                    try
                    {
                        int maxX = posInGrid.Values.Select(w => w.x).Max();
                        int maxY = posInGrid.Values.Select(w => w.y).Max();
                        int minX = posInGrid.Values.Select(w => w.x).Min();
                        int minY = posInGrid.Values.Select(w => w.y).Min();

                        var topLeft = grid[(minX, minY)];
                        var topRight = grid[(maxX, minY)];
                        var bottomLeft = grid[(minX, maxY)];
                        var bottomRight = grid[(maxX, maxY)];

                        return topLeft.Id * topRight.Id * bottomLeft.Id * bottomRight.Id;
                    }
                    catch
                    {

                    }
                }
            }

            return 0;
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
