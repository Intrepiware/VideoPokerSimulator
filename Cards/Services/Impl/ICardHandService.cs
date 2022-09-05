using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Services.Impl
{
    public class ICardHandService : Services.ICardHandService
    {
        public Hand GetHand(List<Card> cards)
        {
            var rankedCards = cards.OrderByDescending(x => x.Rank).ToList();

            Rank? rank = default, rank2 = default;
            if(IsStraightFlush(cards, out rank))
                return new Hand { HandType = HandType.StraightFlush, PrimaryRank = rank.Value };

            if(IsFourOfAKind(cards, out rank))
                return new Hand { HandType = HandType.FourOfAKind, PrimaryRank = rank.Value };

            if(IsFullHouse(cards, out rank, out rank2))
                return new Hand { HandType=HandType.FullHouse, PrimaryRank = rank.Value, SecondaryRank = rank2.Value };

            if(IsFlush(cards))
                return new Hand { HandType = HandType.Flush, PrimaryRank = cards.OrderByDescending(x => x.Rank).First().Rank };

            if(IsStraight(cards, out rank))
                return new Hand { HandType = HandType.Straight, PrimaryRank = rank.Value };

            if(IsThreeOfAKind(cards, out rank))
                return new Hand { HandType = HandType.ThreeOfAKind, PrimaryRank = rank.Value };

            if(IsTwoPair(cards, out rank, out rank2))
                return new Hand { HandType = HandType.TwoPair, PrimaryRank = rank.Value, SecondaryRank = rank2.Value };

            if(IsPair(cards, out rank))
                return new Hand { HandType = HandType.Pair, PrimaryRank = rank.Value };

            return new Hand { HandType = HandType.HighCard, PrimaryRank = cards.OrderByDescending(x => x.Rank).First().Rank };

        }
        public bool IsFlush(List<Card> cards) => cards.GroupBy(x => x.Suit).Count() == 1;
        public bool IsFourOfAKind(List<Card> cards, out Rank? rank)
        {
            SanityCheck(cards);
            rank = null;
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() == 4)
                              .ToList();

            if (groups.Any())
            {
                rank = groups[0].Key;
                return true;
            }
            return false;

        }
        public bool IsPair(List<Card> cards, out Rank? highestPair)
        {
            SanityCheck(cards);
            highestPair = null;
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 2)
                              .OrderByDescending(x => (int)x.First().Rank)
                              .ToList();

            if (groups.Any())
            {
                highestPair = groups[0].Key;
                return true;
            }
            return false;
        }
        public bool IsStraight(List<Card> cards, out Rank? rank)
        {
            SanityCheck(cards);
            rank = null;
            var ranks = cards.OrderBy(x => x.Rank).Select(x => (int)x.Rank).ToList();

            // Check for ace-low straight
            if(!ranks.Except(new[] { 0, 1, 2, 3, 12}).Any())
            {
                rank = Rank.Five;
                return true;
            }

            if(ranks[0] + 1 == ranks[1]
                && ranks[1] + 1 == ranks[2]
                && ranks[2] + 1 == ranks[3]
                && ranks[3] + 1 == ranks[4])
            {
                rank = (Rank)ranks[4];
                return true;
            }

            return false;

        }
        public bool IsStraightFlush(List<Card> cards, out Rank? rank)
        {
            rank = null;
            if (IsFlush(cards) && IsStraight(cards, out rank))
                return true;
            return false;
        }
        public bool IsThreeOfAKind(List<Card> cards, out Rank? rank)
        {
            SanityCheck(cards);
            rank = null;
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 3)
                              .ToList();

            if(groups.Any())
            {
                rank = groups[0].Key;
                return true;
            }

            return false;

        }
        public bool IsTwoPair(List<Card> cards, out Rank? highestPair, out Rank? lowestPair)
        {
            SanityCheck(cards);
            highestPair = null;
            lowestPair = null;

            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 2)
                              .OrderByDescending(x => (int)x.Key)
                              .ToList();

            if (groups.Count == 2)
            {
                highestPair = groups[0].Key;
                lowestPair = groups[1].Key;
                return true;
            }
            return false;
        }

        public bool IsFullHouse(List<Card> cards, out Rank? threeOfAKindRank, out Rank? pairRank)
        {
            threeOfAKindRank = null;
            pairRank = null;

            var groups = cards.GroupBy(x => x.Rank).OrderByDescending(x => x.Count()).ToList();
            if(groups.Count == 2)
            {
                threeOfAKindRank = groups[0].Key;
                pairRank = groups[1].Key;
                return true;
            }

            return false;
        }

        public byte Compare(Hand hand1, Hand hand2) => throw new NotImplementedException();

        private void SanityCheck(List<Card> cards)
        {
            if (cards?.Count != 5)
                throw new ArgumentException("Card list must have exactly five cards");

            var groups = cards.GroupBy(x => x).Where(x => x.Count() > 1).FirstOrDefault();
            if (groups != null)
                throw new ArgumentException($"Cards must be unique ({groups.Key.Rank} of {groups.Key.Suit} listed multiple times)");
        }
    }
}
