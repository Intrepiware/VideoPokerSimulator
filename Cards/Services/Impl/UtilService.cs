using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Services.Impl
{
    public static class UtilService
    {
        static Random random = new Random();

        public static int Score(Hand hand, int wager)
        {
            switch (hand.HandType)
            {
                case HandType.StraightFlush:
                    return hand.PrimaryRank == Rank.Ace ? 250 * wager : 50 * wager;
                case HandType.FourOfAKind:
                    return 25 * wager;
                case HandType.FullHouse:
                    return 9 * wager;
                case HandType.Flush:
                    return 6 * wager;
                case HandType.Straight:
                    return 4 * wager;
                case HandType.ThreeOfAKind:
                    return 3 * wager;
                case HandType.TwoPair:
                    return 2 * wager;
                case HandType.Pair:
                    return hand.PrimaryRank >= Rank.Jack ? wager : 0;
                default:
                    return 0;
            }
        }

        public static string Stringify(List<Card> cards) => cards.Any() ? string.Join(' ', cards) : "{None}";


        public static List<Card> Shuffle(List<Card> excludedCards)
        {
            var cards = Enumerable.Range(0, 52).Select(x => Card.FromInt(x))
                                                .OrderBy(x => random.NextDouble())
                                                .ToList();

            if (excludedCards?.Count > 0)
            {
                cards = cards.Where(x => !excludedCards.Any(y => x.Rank == y.Rank && x.Suit == y.Suit)).ToList();
            }

            return cards;

        }
    }
}
