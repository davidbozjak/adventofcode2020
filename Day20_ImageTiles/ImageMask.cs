using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    class ImageMask
    {
        private readonly List<string> rows = new List<string>();

        public int Height => this.rows.Count;

        public int Width => this.rows[0].Length;

        public ImageMask()
        {

        }

        public void AddRow(string row)
        {
            if (this.rows.Count > 0 && row.Length != this.rows[0].Length)
                throw new Exception();

            rows.Add(row);
        }

        public IEnumerable<(int x, int y)> GetMaskedLocations()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.rows[y][x] != ' ') yield return (x, y);
                }
            }
        }

        public int MaskedLocationsCount => this.GetMaskedLocations().Count();
    }
}
