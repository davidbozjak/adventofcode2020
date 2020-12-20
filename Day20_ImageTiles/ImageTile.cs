using SantasToolbox;
using System;
using System.Linq;

namespace Day20_ImageTiles
{
    class ImageTile : RotatableImage
    {
        public int Id { get; private set;  }

        public long TopLine => this.cachedTopLine.Value;

        public long BottomLine => this.cachedBottomLine.Value;

        public long LeftLine => this.cachedLeftLine.Value;

        public long RightLine => this.cachedRightLine.Value;

        private readonly Cached<long> cachedLeftLine;
        private readonly Cached<long> cachedRightLine;
        private readonly Cached<long> cachedBottomLine;
        private readonly Cached<long> cachedTopLine;

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
                this.originalRows.Add(row.ToCharArray());
                this.rows.Add(row.ToCharArray());
                this.ResetCachedInfo();
            }
        }

        public override string ToString() => this.Id.ToString();

        private void SetId(string row)
        {
            var idString = row[(row.IndexOf(" ") + 1)..^1];
            this.Id = int.Parse(idString);
        }

        enum Edge { Left = 1, Right = 2, Bottom = 3, Top = 4 }
        private long CalculateLine(Edge edge)
        {
            var edgeStr = GetEdgeString(edge);
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

        private char[] GetEdgeString(Edge edge)
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
                return GetColumn(this.rows.Count - 1);
            }
            else throw new Exception();
        }

        protected override void ResetCachedInfo()
        {
            this.cachedLeftLine.Reset();
            this.cachedRightLine.Reset();
            this.cachedBottomLine.Reset();
            this.cachedTopLine.Reset();
        }
    }
}
