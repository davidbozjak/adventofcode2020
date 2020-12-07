using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day7_Bags
{
    class Program
    {
        private static UniqueFactory<string, Bag> bagFactory = new UniqueFactory<string, Bag>(name => new Bag(name));

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<Bag?>("Input.txt", PraseBag);

            var inputs = inputProvider.Where(w => w != null).Cast<Bag>().ToList();

            var santasBag = bagFactory.GetOrCreateInstance("shiny gold bag");

            int topLevelBags = inputs.Count(w => w.CanContainBag(santasBag));
            Console.WriteLine($"Part 1: Bag can be contained by {topLevelBags} top level bags");

            int bagCount = santasBag.ContainedBagsCount;
            Console.WriteLine($"Part 2: Santas bag must contain {bagCount} bags");
        }

        static bool PraseBag(string? input, out Bag? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(input)) return false;

            var subparts = input.Split(new[] { "contain", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLower())
                .Select(w => w.Trim())
                .Select(w => w.EndsWith(".") ? w[..(w.Length - 1)] : w)
                .Select(w => w.EndsWith("s") ? w[..(w.Length - 1)] : w)
                .ToArray();

            var name = subparts[0];

            value = bagFactory.GetOrCreateInstance(name);

            for (int i = 1; i < subparts.Length; i++)
            {
                if (subparts[1] == "no other bag") break;

                var count = int.Parse(subparts[i][..1]);
                var subBagName = subparts[i][2..];

                value.AddRule(bagFactory.GetOrCreateInstance(subBagName), count);
            }

            return true;
        }

        class Bag
        {
            private readonly Cached<int> cachedContainedBagsCount;

            public string BagType { get; }

            public List<(Bag, int)> Bags { get; } = new List<(Bag, int)>();

            public Bag(string bagType)
            {
                this.BagType = bagType;
                this.cachedContainedBagsCount = new Cached<int>(GetContainedBagsCount);
            }

            public void AddRule(Bag bag, int count)
            {
                this.Bags.Add((bag, count));
                this.cachedContainedBagsCount.Reset();
            }

            public bool CanContainBag(Bag bagToMake)
            {
                foreach((Bag bag, _) in Bags)
                {
                    if (bag == bagToMake) return true;

                    if (bag.CanContainBag(bagToMake)) return true;
                }

                return false;
            }

            public int ContainedBagsCount
            {
                get => this.cachedContainedBagsCount.Value;
            }

            private int GetContainedBagsCount() =>
                Bags.Sum(w => w.Item2 + w.Item2 * w.Item1.ContainedBagsCount);
        }
    }
}
