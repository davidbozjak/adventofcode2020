using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SantasToolbox
{
    public interface IWorldObject
    {
        Point Position { get; }

        char CharRepresentation { get; }

        int Z { get; }
    }

    public interface IWorld
    {
        IEnumerable<IWorldObject> WorldObjects { get; }
    }

    public class WorldPrinter
    {
        private readonly bool skipEmptyLines;
        private readonly int frameSize;

        public WorldPrinter(bool skipEmptyLines = true, int frameSize = 5)
        {
            this.skipEmptyLines = skipEmptyLines;
            this.frameSize = frameSize;
        }

        public void Print(IWorld world)
        {
            if (!world.WorldObjects.Any())
            {
                Console.WriteLine("<<< Blank World >>>");
                return;
            }

            int maxX = world.WorldObjects.Max(w => w.Position.X);
            int maxY = world.WorldObjects.Max(w => w.Position.Y);

            int minX = world.WorldObjects.Min(w => w.Position.X);
            int minY = world.WorldObjects.Min(w => w.Position.Y);

            Print(world, minX, maxX, minY, maxY);
        }

        public void Print(IWorld world, IWorldObject objectOfInterest)
        {
            Print(world, objectOfInterest.Position.X - frameSize, objectOfInterest.Position.X + frameSize, objectOfInterest.Position.Y - frameSize, objectOfInterest.Position.Y + frameSize);
        }

        public void Print(IWorld world, int minX, int maxX, int minY, int maxY)
        {
            Console.Clear();

            for (int y = minY; y <= maxY; y++)
            {
                var row = new StringBuilder(new string(Enumerable.Repeat(' ', (maxX - minX) + 1).ToArray()));

                foreach (var track in world.WorldObjects.Where(w => w.Position.Y == y && w.Position.X >= minX && w.Position.X <= maxX).OrderBy(w => w.Z))
                {
                    row[track.Position.X - minX] = track.CharRepresentation;
                }
                
                if (!skipEmptyLines || !string.IsNullOrWhiteSpace(row.ToString()))
                {
                    Console.WriteLine(row);
                }
            }
        }
    }
}
