using SantasToolbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Day21_Allergies
{
    class Program
    {
        static readonly UniqueFactory<string, Ingredient> ingredientBank = new UniqueFactory<string, Ingredient>(str => new Ingredient(str));
        static readonly UniqueFactory<string, Allergen> allergenBank = new UniqueFactory<string, Allergen>(str => new Allergen(str));

        static void Main(string[] args)
        {
            using var inputProvider = new InputProvider<string>("Input.txt", GetString);

            var inputs = inputProvider.ToList();
            var containtsString = " (contains ";

            var foodList = new List<List<Ingredient>>();

            foreach (var input in inputs)
            {
                var contains = input.IndexOf(containtsString);

                var foodString = input[..contains];
                var allergenList = input[(contains + containtsString.Length)..^1];

                var ingredients = foodString.Split(" ");
                var allergensStrings = allergenList.Split(new [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var allergens = allergensStrings.Select(w => allergenBank.GetOrCreateInstance(w)).ToList();

                var food = new List<Ingredient>();
                foodList.Add(food);

                foreach (var ingredientStr in ingredients)
                {
                    var ingredient = ingredientBank.GetOrCreateInstance(ingredientStr);
                    ingredient.AddPossibleAllergens(allergens);
                    food.Add(ingredient);
                }
            }

            List<Allergen> freeFloatingAllergens;

            do
            {
                freeFloatingAllergens = ingredientBank.AllCreatedInstances.SelectMany(w => w.PossibleAllergens)
                    .ToHashSet()
                    .ToList();

                foreach (var allergen in freeFloatingAllergens)
                {
                    var possibleIngredients = ingredientBank.AllCreatedInstances
                        .Select(w => new { Allergen = w, AllergenSigns = w.GetSignsForAllergen(allergen) })
                        .OrderByDescending(w => w.AllergenSigns)
                        .ToList();

                    if (possibleIngredients[0].AllergenSigns > possibleIngredients[1].AllergenSigns)
                    {
                        possibleIngredients[0].Allergen.ConfirmAllergen(allergen);

                        foreach (var food in ingredientBank.AllCreatedInstances)
                        {
                            food.RemoveAllergen(allergen);
                        }
                    }
                }

            } while (freeFloatingAllergens.Count > 0);

            var allergenFreeIngredients = ingredientBank.AllCreatedInstances
                .Where(w => w.ConfirmedAllergen == null)
                .Where(w => w.PossibleAllergenCount == 0)
                .ToList();

            var count = 0;
            
            foreach (var food in foodList)
            {
                foreach (var ingredient in allergenFreeIngredients)
                {
                    if (food.Contains(ingredient))
                        count++;
                }
            }

            Console.WriteLine($"Part 1: {count}");

            var confirmedAllergenIngredients = ingredientBank.AllCreatedInstances
                .Where(w => w.ConfirmedAllergen != null)
                .OrderBy(w => w.ConfirmedAllergen.Name);

            var canonicalDangerousIngredientList = string.Join(",", confirmedAllergenIngredients);

            Console.WriteLine($"Part 2: canonical dangerous ingredient list: {canonicalDangerousIngredientList}");
        }

        static bool GetString(string? input, out string value)
        {
            value = input ?? string.Empty;

            return !string.IsNullOrWhiteSpace(input);
        }

        class Ingredient
        {
            private readonly Dictionary<Allergen, int> allergens = new Dictionary<Allergen, int>();

            public string Name { get; }

            public Allergen? ConfirmedAllergen { get; private set; }

            public int PossibleAllergenCount => this.allergens.Count;

            public IReadOnlyCollection<Allergen> PossibleAllergens => this.allergens.Keys.ToList().AsReadOnly();

            public Ingredient(string name)
            {
                this.Name = name;
            }

            public void AddPossibleAllergens(IEnumerable<Allergen> possibleA)
            {
                foreach (var allergen in possibleA)
                {
                    if (!this.allergens.ContainsKey(allergen))
                    {
                        this.allergens[allergen] = 0;
                    }

                    this.allergens[allergen]++;
                }
            }

            public bool PossiblyContains(Allergen allergen)
            {
                return this.allergens.Keys.Contains(allergen);
            }

            public int GetSignsForAllergen(Allergen allergen)
            {
                if (this.allergens.ContainsKey(allergen))
                {
                    return this.allergens[allergen];
                }
                else return 0;
            }

            public void ConfirmAllergen(Allergen allergen)
            {
                if (!this.allergens.Keys.Contains(allergen))
                    throw new Exception("Broken assumption");

                this.ConfirmedAllergen = allergen;

                this.allergens.Clear();
            }

            public void RemoveAllergen(Allergen allergen)
            {
                this.allergens.Remove(allergen);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        class Allergen
        {
            public string Name { get; }

            public Allergen(string name)
            {
                this.Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
