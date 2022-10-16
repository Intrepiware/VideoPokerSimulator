using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cards.Services.Impl
{
    public static class UtilService
    {
        // https://stackoverflow.com/a/38530913
        private static int _tracker = 0;

        private static ThreadLocal<Random> _random = new ThreadLocal<Random>(() => {
            var seed = (int)(Environment.TickCount & 0xFFFFFF00 | (byte)(Interlocked.Increment(ref _tracker) % 255));
            var random = new Random(seed);
            return random;
        });

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
                                                .ToList();

            if (excludedCards?.Count > 0)
            {
                cards = cards.Where(x => !excludedCards.Any(y => x.Rank == y.Rank && x.Suit == y.Suit)).ToList();
            }

            return cards.OrderBy(x => _random.Value.NextDouble()).ToList();

        }
    }
}
