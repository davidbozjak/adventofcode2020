using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Day22_RecursiveCombat
{
    class Program
    {
        static void Main(string[] args)
        {
            using var player1inputProvider = new InputProvider<int>("Player1.txt", GetInt);
            using var player2inputProvider = new InputProvider<int>("Player2.txt", GetInt);

            Part1(player1inputProvider, player2inputProvider);

            player1inputProvider.Reset();
            player2inputProvider.Reset();

            Part2(player1inputProvider, player2inputProvider);
        }

        static void Part1(IEnumerable<int> player1inputProvider, IEnumerable<int> player2inputProvider)
        {
            var player1 = new Deck("Player 1", player1inputProvider);
            var player2 = new Deck("Player 2", player2inputProvider);

            for (int round = 0; player1.CardCount > 0 && player2.CardCount > 0; round++)
            {
                var player1Card = player1.TakeCard();
                var player2Card = player2.TakeCard();

                var winningCard = player1Card > player2Card ? player1Card : player2Card;
                var losingCard = player1Card < player2Card ? player1Card : player2Card;

                var winner = player1Card > player2Card ? player1 : player2;
                winner.AddCard(winningCard);
                winner.AddCard(losingCard);
            }

            Console.WriteLine($"Part 1: Winner score: {player1.GetScore()} {player2.GetScore()}");
        }

        static void Part2(IEnumerable<int> player1inputProvider, IEnumerable<int> player2inputProvider)
        {
            var player1Initial = new Deck("Player 1", player1inputProvider);
            var player2Initial = new Deck("Player 2", player2inputProvider);

            RecursiveCombat(player1Initial, player2Initial);

            Console.WriteLine($"Part 2: Winner score: {player1Initial.GetScore()} {player2Initial.GetScore()}");

            static int RecursiveCombat(Deck player1, Deck player2)
            {
                var previousPlayer1Configurations = new HashSet<long>();
                var previousPlayer2Configurations = new HashSet<long>();

                for (int round = 0; player1.CardCount > 0 && player2.CardCount > 0; round++)
                {
                    var player1Config = player1.GetConfigurationSignature();
                    var player2Config = player2.GetConfigurationSignature();

                    if (previousPlayer1Configurations.Contains(player1Config) ||
                    previousPlayer2Configurations.Contains(player2Config))
                    {
                        //Console.WriteLine($"Game ends because of recursion rule after round {round} Player 1 score: {player1.GetScore()}");
                        return 1;
                    }

                    previousPlayer1Configurations.Add(player1Config);
                    previousPlayer2Configurations.Add(player2Config);

                    var player1Card = player1.TakeCard();
                    var player2Card = player2.TakeCard();

                    Deck winner;
                    int winningCard;
                    int losingCard;

                    if (player1.CardCount >= player1Card && player2.CardCount >= player2Card)
                    {
                        var resultOfRecursiveCombat = RecursiveCombat(player1.CopyDeck(player1Card), player2.CopyDeck(player2Card));

                        winner = resultOfRecursiveCombat > 0 ? player1 : player2;
                        winningCard = resultOfRecursiveCombat > 0 ? player1Card : player2Card;
                        losingCard = resultOfRecursiveCombat < 0 ? player1Card : player2Card;
                    }
                    else
                    {
                        winningCard = player1Card > player2Card ? player1Card : player2Card;
                        losingCard = player1Card < player2Card ? player1Card : player2Card;

                        winner = player1Card > player2Card ? player1 : player2;
                    }

                    winner.AddCard(winningCard);
                    winner.AddCard(losingCard);
                }

                return player1.CardCount > 0 ? 1 : -1;
            }
        }

        static bool GetInt(string? input, out int value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(input)) return false;

            try
            {
                value = int.Parse(input);
            }
            catch
            {
                return false;
            }

            return true;
        }

        class Deck
        {
            private readonly List<int> cards = new List<int>();

            public string PlayerName { get;  }

            public int CardCount => this.cards.Count;

            public IReadOnlyCollection<int> Cards => this.cards.AsReadOnly();

            public Deck(string playerName, IEnumerable<int> startingDeck)
            {
                this.PlayerName = playerName;

                foreach (var card in startingDeck)
                {
                    this.cards.Add(card);
                }
            }

            public Deck CopyDeck(int nTopCards)
            {
                return new Deck(PlayerName, this.cards.Take(nTopCards).ToList());
            }

            public void AddCard(int card)
            {
                this.cards.Add(card);
            }

            public int TakeCard()
            {
                var card = this.cards[0];
                this.cards.RemoveAt(0);

                return card;
            }

            public int GetScore()
            {
                int score = 0;

                for (int i = this.cards.Count - 1; i >= 0; i--)
                {
                    score += (this.cards.Count - i) * this.cards[i];
                }

                return score;
            }

            public long GetConfigurationSignature()
            {
                long score = 0;
                long place = 1;

                for (int i = 0; i < this.cards.Count; i++)
                {
                    score += place * this.cards[i];
                    place *= 10;
                }

                return score;
            }
        }
    }
}
