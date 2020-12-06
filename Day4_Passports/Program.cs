using SantasToolbox;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4_Passports
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new MultiLineParser<Passport>(() => new Passport(), UpdatePassportWithValues);
            using var inputProvider = new InputProvider<Passport?>("Input.txt", parser.AddLine)
            {
                EndAtEmptyLine = false
            };

            var inputs = inputProvider.ToList();
            var passports = inputs.Where(w => w != null).Cast<Passport>().ToList();

            Console.WriteLine($"Number of passports: {passports.Count}");

            // Part 1            
            Console.WriteLine($"Part1: Number of valid passports: {passports.Count(w => w.IsValid(false))}");
            Console.WriteLine($"Part2: Number of valid passports: {passports.Count(w => w.IsValid(true))}");
        }

        private static void UpdatePassportWithValues(Passport passport, string input)
        {
            passport.BirthYear = ParseKeyFromFileOrNull(input, "byr") ?? passport.BirthYear;
            passport.IssueYear = ParseKeyFromFileOrNull(input, "iyr") ?? passport.IssueYear;
            passport.ExpirationYear = ParseKeyFromFileOrNull(input, "eyr") ?? passport.ExpirationYear;
            passport.Height = ParseKeyFromFileOrNull(input, "hgt") ?? passport.Height;
            passport.HairColor = ParseKeyFromFileOrNull(input, "hcl") ?? passport.HairColor;
            passport.EyeColor = ParseKeyFromFileOrNull(input, "ecl") ?? passport.EyeColor;
            passport.PassportID = ParseKeyFromFileOrNull(input, "pid") ?? passport.PassportID;
            passport.CountryID = ParseKeyFromFileOrNull(input, "cid") ?? passport.CountryID;
        }

        private static string? ParseKeyFromFileOrNull(string input, string key)
        {
            if (!input.Contains(key)) return null;

            int startIndex = input.IndexOf(key) + key.Length + 1;
            int stopIndex = input.IndexOf(" ", startIndex);
            int length = (stopIndex > startIndex ? stopIndex : input.Length) - startIndex;

            return input.Substring(startIndex, length);
        }

        class Passport
        {
            private static readonly Regex numRegex = new Regex(@"\d+");
            private static readonly Regex hexColorRegex = new Regex(@"#[0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z]");

            public string? BirthYear { get; set; }
            public string? IssueYear { get; set; }
            public string? ExpirationYear { get; set; }
            public string? Height { get; set; }
            public string? HairColor { get; set; }
            public string? EyeColor { get; set; }
            public string? PassportID { get; set; }
            public string? CountryID { get; set; }

            public bool IsValid(bool applyBusinessLogic)
            {
                if (string.IsNullOrWhiteSpace(this.BirthYear)) return false;
                else if (applyBusinessLogic)
                {
                    if (int.TryParse(this.BirthYear, out int year))
                    {
                        if (year < 1920 || year > 2002)
                            return false;
                    }
                    else return false;
                }

                if (string.IsNullOrWhiteSpace(this.IssueYear)) return false;
                else if (applyBusinessLogic)
                {
                    if (int.TryParse(this.IssueYear, out int year))
                    {
                        if (year < 2010 || year > 2020)
                            return false;
                    }
                    else return false;
                }

                if (string.IsNullOrWhiteSpace(this.ExpirationYear)) return false;
                else if (applyBusinessLogic)
                {
                    if (int.TryParse(this.ExpirationYear, out int year))
                    {
                        if (year < 2020 || year > 2030)
                            return false;
                    }
                    else return false;
                }

                if (string.IsNullOrWhiteSpace(this.Height)) return false;
                else if (applyBusinessLogic)
                {
                    var numMatches = numRegex.Matches(this.Height).Select(w => int.Parse(w.Value)).ToArray();

                    if (numMatches.Length != 1) return false;
                    int height = numMatches[0];

                    if (this.Height.Contains("cm"))
                    {
                        if (height < 150 || height > 193) return false;
                    }
                    else if (this.Height.Contains("in"))
                    {
                        if (height < 59 || height > 76) return false;
                    }
                    else return false;
                }

                if (string.IsNullOrWhiteSpace(this.HairColor)) return false;
                else if (applyBusinessLogic)
                {
                    if (!hexColorRegex.IsMatch(this.HairColor)) return false;
                }

                if (string.IsNullOrWhiteSpace(this.EyeColor)) return false;
                else if (applyBusinessLogic)
                {
                    if (this.EyeColor != "amb" &&
                        this.EyeColor != "blu" &&
                        this.EyeColor != "brn" &&
                        this.EyeColor != "gry" &&
                        this.EyeColor != "grn" &&
                        this.EyeColor != "hzl" &&
                        this.EyeColor != "oth")
                        return false;
                }

                if (string.IsNullOrWhiteSpace(this.PassportID)) return false;
                else if (applyBusinessLogic)
                {
                    if (!numRegex.IsMatch(this.PassportID)) return false;
                    if (this.PassportID.Length != 9) return false;
                }

                return true;
            }
        }
    }
}
