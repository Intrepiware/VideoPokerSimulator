using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Services.Impl
{
    public class CardHandService : ICardHandService
    {
        public Hand GetHand(List<Card> cards)
        {
            var rankedCards = cards.OrderByDescending(x => x.Rank).ToList();

            Rank? rank = default, rank2 = default;
            if(GetStraightFlush(cards, out rank))
                return new Hand { HandType = HandType.StraightFlush, PrimaryRank = rank.Value };

            if(GetFourOfAKind(cards, out rank))
                return new Hand { HandType = HandType.FourOfAKind, PrimaryRank = rank.Value };

            if(GetFullHouse(cards, out rank, out rank2) != null)
                return new Hand { HandType=HandType.FullHouse, PrimaryRank = rank.Value, SecondaryRank = rank2.Value };

            if(GetFlush(cards))
                return new Hand { HandType = HandType.Flush, PrimaryRank = cards.OrderByDescending(x => x.Rank).First().Rank };

            if(GetStraight(cards, out rank))
                return new Hand { HandType = HandType.Straight, PrimaryRank = rank.Value };

            if(GetThreeOfAKind(cards, out rank))
                return new Hand { HandType = HandType.ThreeOfAKind, PrimaryRank = rank.Value };

            if(GetTwoPair(cards) is {} twoPair)
            {
                var pairs = twoPair.GroupBy(x => x.Rank)
                                .Where(x => x.Count() == 2)
                                .OrderByDescending(x => x.Key)
                                .ToList();
                return new Hand { HandType = HandType.TwoPair, PrimaryRank = pairs[0].Key, SecondaryRank = pairs[1].Key };
            }

            if(GetPair(cards) is {} pair)
            {
                rank = pair.GroupBy(x => x.Rank)
                            .Where(x => x.Count() >= 2)
                            .OrderByDescending(x => x.Key)
                            .First().Key;
                return new Hand { HandType = HandType.Pair, PrimaryRank = rank.Value };
            }

            return new Hand { HandType = HandType.HighCard, PrimaryRank = cards.OrderByDescending(x => x.Rank).First().Rank };

        }
        public bool GetFlush(List<Card> cards)
        {
            SanityCheck(cards);
            return cards.GroupBy(x => x.Suit).Count() == 1;
        }
        public bool GetFourOfAKind(List<Card> cards, out Rank? rank)
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
        public List<Card> GetPair(List<Card> cards)
        {
            SanityCheck(cards);
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 2)
                              .OrderByDescending(x => (int)x.First().Rank)
                              .ToList();

            if (groups.Any())
            {
                return groups[0].Take(2).ToList();
            }
            return null;
        }
        public bool GetStraight(List<Card> cards, out Rank? rank)
        {
            SanityCheck(cards);
            rank = null;
            var ranks = cards.OrderBy(x => x.Rank).Select(x => (int)x.Rank).ToList();

            // Check for ace-low straight
            if(!(new[] { 0, 1, 2, 3, 12 }.Except(ranks).Any()))
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
        public bool GetStraightFlush(List<Card> cards, out Rank? rank)
        {
            rank = null;
            if (GetFlush(cards) && GetStraight(cards, out rank))
                return true;
            return false;
        }
        public bool GetThreeOfAKind(List<Card> cards, out Rank? rank)
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
        public List<Card> GetTwoPair(List<Card> cards)
        {
            SanityCheck(cards);

            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 2)
                              .OrderByDescending(x => x.Key)
                              .ToList();

            if (groups.Count >= 2)
            {
                var output = groups[0].Take(2).ToList();
                output.AddRange(groups[1].Take(2));
                return output;
            }
            return null;
        }

        public List<Card> GetFullHouse(List<Card> cards, out Rank? threeOfAKindRank, out Rank? pairRank)
        {
            SanityCheck(cards);
            threeOfAKindRank = null;
            pairRank = null;

            var groups = cards.GroupBy(x => x.Rank).ToList();
            if(groups.Count >= 2)
            {
                var threeOfAKind = groups.Where(x => x.Count() >= 3)
                                        .OrderByDescending(x => x.Key)
                                        .FirstOrDefault()?
                                        .Take(3)
                                        .ToList();

                if(threeOfAKind != null)
                {
                    var pair = groups.Where(x => x.Count() >= 2 && x.Key != threeOfAKind.First().Rank)
                                    .OrderByDescending(x => x.Key)
                                    .FirstOrDefault()?
                                    .Take(2)
                                    .ToList();

                    if(pair != null)
                    {
                        threeOfAKindRank = threeOfAKind.First().Rank;
                        pairRank = pair.First().Rank;
                        return threeOfAKind.Concat(pair).ToList();
                    }
                }
            }

            return null;
        }

        public byte Compare(Hand hand1, Hand hand2) => throw new NotImplementedException();

        private void SanityCheck(List<Card> cards)
        {
            if (cards?.Count < 5 || cards?.Count > 7)
                throw new ArgumentException("Card list must have between five and seven cards");

            var groups = cards.GroupBy(x => new { x.Rank, x.Suit }).Where(x => x.Count() > 1).FirstOrDefault();
            if (groups != null)
                throw new ArgumentException($"Cards must be unique ({groups.Key.Rank} of {groups.Key.Suit} listed multiple times)");
        }
    }
}
