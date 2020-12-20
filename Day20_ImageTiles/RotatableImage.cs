using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day20_ImageTiles
{
    abstract class RotatableImage
    {
        protected readonly List<char[]> originalRows = new List<char[]>();
        protected readonly List<char[]> rows = new List<char[]>();
        
        private int rotation = 0;
        private bool isFlippedHorizontally = false;
        private bool isFlippedVerticallly = false;

        public int Rotation => this.rotation;

        public int Size => this.rows.Count;

        public void Rotate90()
        {
            this.rotation++;
            while (this.rotation < 0) this.rotation += 4;
            while (this.rotation >= 4) this.rotation -= 4;

            var newRows = new List<char[]>();

            for (int i = 0; i < this.rows.Count; i++)
            {
                newRows.Add(GetColumn(i));
            }

            this.rows.Clear();
            this.rows.AddRange(newRows);

            this.ResetCachedInfo();
        }

        public void SetRotateSteps(int steps)
        {
            while (steps < 0) steps += 4;
            while (steps >= 4) steps -= 4;

            for (int i = this.rotation; i <= steps; i++)
            {
                this.Rotate90();
            }

            this.ResetCachedInfo();
        }

        public void Reset()
        {
            this.rows.Clear();
            this.rows.AddRange(this.originalRows.Select(w => w.ToArray()));

            this.rotation = 0;
            this.ResetCachedInfo();
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

        private void FlipHorizontal()
        {
            var flippedRows = new List<char[]>();

            for (int i = 0; i < this.rows.Count; i++)
            {
                flippedRows.Add(this.rows[i].Reverse().ToArray());
            }

            this.rows.Clear();
            this.rows.AddRange(flippedRows);

            this.isFlippedHorizontally = !this.isFlippedHorizontally;
            this.ResetCachedInfo();
        }

        private void FlipVertical()
        {
            this.rows.Reverse();
            
            this.isFlippedVerticallly = !this.isFlippedVerticallly;
            this.ResetCachedInfo();
        }

        public char[] GetColumn(int column)
        {
            return this.rows.Select(w => w[column]).ToArray();
        }

        public char[] GetRow(int row) =>
            this.rows[row];

        protected abstract void ResetCachedInfo();
    }
}
