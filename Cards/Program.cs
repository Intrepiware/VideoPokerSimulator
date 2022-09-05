using Cards.Models;
using Cards.Services.Impl;
using System;
using System.Linq;

namespace Cards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var handService = new CardHandService();


            while (true)
            {
                for (long i = 0; ; i++)
                {
                    var cards = Enumerable.Range(0, 51).Select(x => Card.FromInt(x))
                                                        .OrderBy(x => random.NextDouble())
                                                        .Take(5)
                                                        .ToList();

                    var hand = handService.GetHand(cards);

                    if (hand.HandType > HandType.Pair
                        || (hand.HandType == HandType.Pair && hand.PrimaryRank >= Rank.Jack))
                    {
                        Console.WriteLine($"\n\n{cards[0]} {cards[1]} {cards[2]} {cards[3]} {cards[4]}\nFound {hand.HandType}\nIterations: {i + 1}\nPrimary Rank: {hand.PrimaryRank}\nSecondary Rank: {hand.SecondaryRank}");
                        break;
                    }
                }

                Console.Write("Go again? [Y/N]: ");
                if (Console.ReadKey().KeyChar.ToString().ToUpper() != "Y")
                    break;
            }
        }
    }
}
