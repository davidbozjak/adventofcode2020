using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    partial class Program
    {
        class ImageTile
        {
            public int Id { get; private set;  }

            public long TopLine => this.cachedTopLine.Value;
            public string TopLineStr => this.GetEdgeString(Edge.Top);

            public long BottomLine => this.cachedBottomLine.Value;
            public string BottomLineStr => this.GetEdgeString(Edge.Bottom);

            public long LeftLine => this.cachedLeftLine.Value;
            public string LeftLineStr => this.GetEdgeString(Edge.Left);

            public long RightLine => this.cachedRightLine.Value;
            public string RightLineStr => this.GetEdgeString(Edge.Right);

            private readonly List<string> originalRows = new List<string>();
            private readonly List<string> rows = new List<string>();

            private readonly Cached<long> cachedLeftLine;
            private readonly Cached<long> cachedRightLine;
            private readonly Cached<long> cachedBottomLine;
            private readonly Cached<long> cachedTopLine;

            private int rotation = 0;
            private bool isFlippedHorizontally = false;
            private bool isFlippedVerticallly = false;
            private bool isFrozen = false;
            private int sideLength = 0;

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
                    this.originalRows.Add(row);
                    this.rows.Add(row);
                    this.sideLength++;
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
                while (this.rotation < 0) this.rotation += 4;
                while (this.rotation >= 4) this.rotation -= 4;

                var newRows = new List<string>();

                for (int i = 0; i < this.rows.Count; i++)
                {
                    newRows.Add(GetColumn(i));
                }

                this.rows.Clear();
                this.rows.AddRange(newRows);

                this.ResetLines();
            }

            public void SetRotateSteps(int steps)
            {
                if (this.isFrozen) throw new Exception();

                while (steps < 0) steps += 4;
                while (steps >= 4) steps -= 4;

                for (int i = this.rotation; i <= steps; i++)
                {
                    this.Rotate90();
                }

                this.ResetLines();
            }

            public int Rotation => this.rotation;

            public void Reset()
            {
                this.rows.Clear();
                this.rows.AddRange(this.originalRows);

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
                if (edge == Edge.Top)
                {
                    return this.rows[0];
                }
                else if (edge == Edge.Bottom)
                {
                    return this.rows.Last();
                }
                else if (edge == Edge.Left)
                {
                    return GetColumn(0);
                }
                else if (edge == Edge.Right)
                {
                    return GetColumn(this.sideLength - 1);
                }
                else throw new Exception();
            }

            private string GetColumn(int column)
            {
                return new string(this.rows.Select(w => w[column]).ToArray());
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
