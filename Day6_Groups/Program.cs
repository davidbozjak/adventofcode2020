using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day6_Groups
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new MultiLineParser<Group>(() => new Group(), (g, v) => g.Add(v));
            using var inputProvider = new InputProvider<Group?>("Input.txt", parser.AddLine)
            {
                EndAtEmptyLine = false
            };

            var groups = inputProvider.Where(w => w != null).Cast<Group>().ToList();

            var sumUnion = groups.Sum(w => w.CountOfGroupUnion);
            Console.WriteLine($"Part 1: Sum of groups unions: {sumUnion}");

            var sumIntersection = groups.Sum(w => w.CountOfGroupIntersection);
            Console.WriteLine($"Part 2: Sum of groups intersections: {sumIntersection}");
        }

        class Group
        {
            private readonly List<string> allLines = new List<string>();
            private readonly Cached<int> countUnion;
            private readonly Cached<int> countIntersect;

            public Group()
            {
                this.countUnion = new Cached<int>(GetCountOfGroupUnion);
                this.countIntersect = new Cached<int>(GetCountOfGroupIntersection);
            }

            public void Add(string value)
            {
                this.allLines.Add(value);
                this.countUnion.Reset();
                this.countIntersect.Reset();
            }

            public int CountOfGroupUnion
            {
                get => this.countUnion.Value;
            }

            public int CountOfGroupIntersection
            {
                get => this.countIntersect.Value;
            }

            private int GetCountOfGroupUnion()
            {
                IEnumerable<char> intersection = this.allLines[0];

                for (int i = 1; i < this.allLines.Count; i++)
                {
                    intersection = intersection.Union(this.allLines[i]);
                }

                return intersection.Count();
            }

            private int GetCountOfGroupIntersection()
            {
                IEnumerable<char> intersection = this.allLines[0];

                for (int i = 1; i < this.allLines.Count; i++)
                {
                    intersection = intersection.Intersect(this.allLines[i]);
                }

                return intersection.Count();
            }
        }
    }
}
