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

            AssembleTilesToImage(tiles);
        }

        private static long AssembleTilesToImage(IEnumerable<ImageTile> tiles)
        {
            var tilesToAlign = tiles.Skip(1).ToList();
            var alignedTiles = new List<ImageTile>();
            alignedTiles.Add(tiles.First());

            while (tilesToAlign.Count > 0)
            {
                for (int i = 0; i < tilesToAlign.Count; i++)
                {
                    var tile = tilesToAlign[i];

                    int transformCount = 0;
                    long startingTopEdge = tile.TopLine;
                    bool foundMatch = true;
                    while (!MatchesAnyAlignedEdge(tile))
                    {
                        transformCount++;
                        if (transformCount % 4 == 0 || transformCount % 6 == 0)
                        {
                            tile.Flip();
                        }

                        tile.Rotate90();

                        // todo: change this to rely only on transformCount
                        if (tile.TopLine == startingTopEdge)
                        {
                            foundMatch = false;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        alignedTiles.Add(tile);
                        tilesToAlign.Remove(tile);
                    }
                }
            }

            var topLeftTileCount = alignedTiles
                .Where(w => !alignedTiles.Any(ww => ww.BottomLine == w.TopLine))
                .Where(w => !alignedTiles.Any(ww => ww.RightLine == w.LeftLine))
                .Count();

            return 0;

            bool MatchesAnyAlignedEdge(ImageTile t)
            {
                foreach (var at in alignedTiles)
                {
                    if (at.TopLine == t.BottomLine) return true;
                    if (at.BottomLine == t.TopLine) return true;
                    if (at.LeftLine == t.RightLine) return true;
                    if (at.RightLine == t.LeftLine) return true;
                }

                return false;
            }
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

            public void Flip()
            {
                var flippedRows = new List<string>();

                for (int i = 0; i < this.rows.Count; i++)
                {
                    flippedRows.Add(new string(this.rows[i].Reverse().ToArray()));
                }

                this.rows.Clear();
                this.rows.AddRange(flippedRows);

                this.ResetLines();
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
                while ((int)edgeAfterTotation < 0) edgeAfterTotation += 4;

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
