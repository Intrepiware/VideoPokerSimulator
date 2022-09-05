using Cards.Models;
using System;
using System.Linq;

namespace Cards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var cards = Enumerable.Range(0, 51)
                                    .Select(x => new { Card = Card.FromInt(x), Random = random.NextDouble() })
                                    //.OrderBy(x => x.Random)
                                    .Select(x => x.Card);

            foreach (var card in cards)
                Console.WriteLine($"{card.Rank} of {card.Suit}");
        }
    }
}
