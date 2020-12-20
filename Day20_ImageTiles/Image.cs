using System;
using System.Collections.Generic;
using System.Linq;

namespace Day20_ImageTiles
{
    class Image : RotatableImage
    {
        private readonly int sideLength;

        public Image(int tilesPerRow, ImageTile sampleTile)
        {
            this.sideLength = tilesPerRow * (sampleTile.Size - 2);

            for (int i = 0; i < this.sideLength; i++)
            {
                this.rows.Add(new char[this.sideLength]);
                this.originalRows.Add(new char[this.sideLength]);
            }
        }

        public void Insert(ImageTile image, int sampleX, int sampleY)
        {
            int refX = sampleX * (image.Size - 2);
            int refY = sampleY * (image.Size - 2);

            for (int y = 1; y <= image.Size - 2; y++)
            {
                var rowToCopy = image.GetRow(y);

                for (int x = 1; x <= image.Size - 2; x++)
                {
                    this.rows[refY + y - 1][refX + x - 1] = rowToCopy[x];
                    this.originalRows[refY + y - 1][refX + x - 1] = rowToCopy[x];
                }
            }
        }

        public int Count(Func<char, bool> predicate)
        {
            int count = 0;

            for (int y = 0; y < this.Size; y++)
            {
                for (int x = 0; x < this.Size; x++)
                {
                    if (predicate(this.rows[y][x]))
                        count++;
                }
            }

            return count;
        }

        public List<(int x, int y)> FindMaskWithinImage(ImageMask mask)
        {
            var list = new List<(int x, int y)>();

            for (int baseY = 0; baseY < this.Size - mask.Height; baseY++)
            {
                for (int baseX = 0; baseX < this.Size - mask.Width; baseX++)
                {
                    bool foundMonster = true;

                    foreach ((int offsetX, int offsetY) in mask.GetMaskedLocations())
                    {
                        int x = baseX + offsetX;
                        int y = baseY + offsetY;

                        if (this.rows[y][x] != '#')
                        {
                            foundMonster = false;
                            break;
                        }
                    }

                    if (foundMonster)
                    {
                        list.Add((baseX, baseY));
                    }
                }
            }

            return list;
        }

        public void PrintImageWithinImage(ImageMask mask, IList<(int x, int y)> locations, Action<string> printRowAction)
        {
            var dirtyRows = new List<char[]>();
            dirtyRows.AddRange(this.rows.Select(w => w.ToArray()));

            foreach ((int baseX, int baseY) in locations) 
            {
                foreach ((int offsetX, int offsetY) in mask.GetMaskedLocations())
                {
                    int x = baseX + offsetX;
                    int y = baseY + offsetY;

                    dirtyRows[y][x] = 'O';
                }
            }

            foreach (var row in dirtyRows)
            {
                printRowAction(new string(row));
            }
        }

        protected override void ResetCachedInfo()
        {
            
        }
    }
}
