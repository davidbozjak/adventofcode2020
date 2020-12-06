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
            var parser = new MultiLineParser();
            using var inputProvider = new InputProvider<Group?>("Input.txt", parser.AddLine)
            {
                EndAtEmptyLine = false
            };

            var groups = inputProvider.Where(w => w != null).Cast<Group>().ToList();

            var sumUnion = groups.Sum(w => w.CountOfGroupUnion);
            Console.WriteLine($"Part 1: Sum of groups unions: {sumUnion}");

            var sumIntersection = groups.Sum(w => w.CountOfGroupIntersection);
            Console.WriteLine($"Part 1: Sum of groups intersections: {sumIntersection}");
        }

        class Group
        {
            private readonly List<string> allLines = new List<string>();

            public void AddLine(string value)
            {
                this.allLines.Add(value);
            }

            public int CountOfGroupUnion
            {
                get => GetCountOfGroupUnion();
            }

            public int CountOfGroupIntersection
            {
                get => GetCountOfGroupIntersection();
            }

            private int GetCountOfGroupUnion()
            {
                HashSet<char> set = new HashSet<char>();

                foreach (var line in this.allLines)
                {
                    foreach (var c in line)
                    {
                        set.Add(c);
                    }
                }

                return set.Count;
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

        class MultiLineParser
        {
            private Group? currentGroup = null;

            public bool AddLine(string? input, out Group? value)
            {
                value = null;

                if (input == null)
                {
                    if (currentGroup != null)
                    {
                        value = currentGroup;
                        currentGroup = null;
                        return true;
                    }
                    else return false;
                }

                if (string.IsNullOrWhiteSpace(input) && currentGroup == null)
                    return false;

                if (string.IsNullOrWhiteSpace(input) && currentGroup != null)
                {
                    value = currentGroup;
                    currentGroup = null;
                    return true;
                }

                if (currentGroup == null)
                {
                    currentGroup = new Group();
                }

                currentGroup.AddLine(input);

                return true;
            }
        }
    }
}
