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

            if (GetStraightFlush(cards) is { } straightFlush)
            {
                if (straightFlush.Max(x => x.Rank) == Rank.Ace && straightFlush.Min(x => x.Rank) == Rank.Two)
                    return new Hand { HandType = HandType.StraightFlush, PrimaryRank = Rank.Five };
                return new Hand { HandType = HandType.StraightFlush, PrimaryRank = straightFlush.Max(x => x.Rank) };
            }

            if(GetFourOfAKind(cards) is {} fourOfKind)
                return new Hand { HandType = HandType.FourOfAKind, PrimaryRank = fourOfKind.First().Rank };

            if (GetFullHouse(cards) is { } fullHouse)
            {
                var groups = fullHouse.GroupBy(x => x.Rank)
                                        .OrderByDescending(x => x.Count())
                                        .ToList();

                return new Hand { HandType = HandType.FullHouse, PrimaryRank = groups[0].Key, SecondaryRank = groups[1].Key };
            }

            if(GetFlush(cards) is {} flush)
                return new Hand { HandType = HandType.Flush, PrimaryRank = flush.Max(x => x.Rank) };

            if (GetStraight(cards) is {} straight)
            {
                // Check for ace-low straight
                if(straight.Max(x => x.Rank) == Rank.Ace && straight.Min(x => x.Rank) == Rank.Two)
                    return new Hand { HandType = HandType.Straight, PrimaryRank = Rank.Five };

                return new Hand { HandType = HandType.Straight, PrimaryRank = straight.Max(x => x.Rank) };
            }

            if(GetThreeOfAKind(cards) is {} threeOfKind)
                return new Hand { HandType = HandType.ThreeOfAKind, PrimaryRank = threeOfKind[0].Rank };

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
                var rank = pair.GroupBy(x => x.Rank)
                            .Where(x => x.Count() >= 2)
                            .OrderByDescending(x => x.Key)
                            .First().Key;
                return new Hand { HandType = HandType.Pair, PrimaryRank = rank };
            }

            return new Hand { HandType = HandType.HighCard, PrimaryRank = cards.OrderByDescending(x => x.Rank).First().Rank };

        }
        public List<Card> GetFlush(List<Card> cards)
        {
            SanityCheck(cards);
            return cards.GroupBy(x => x.Suit)
                        .FirstOrDefault(x => x.Count() >= 5)?
                        .ToList();
                        
        }
        public List<Card> GetFourOfAKind(List<Card> cards)
        {
            SanityCheck(cards);
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() == 4)
                              .ToList();

            if (groups.Any())
            {
                return groups[0].ToList();
            }
            return null;

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
        public List<Card> GetStraight(List<Card> cards)
        {
            SanityCheck(cards);

            var clone = cards.GroupBy(x => x.Rank)
                            .Select(x => new Card(x.First().Rank, x.First().Suit))
                            .OrderByDescending(x => x.Rank)
                            .ToList();

            while(clone.Count >= 5)
            {
                if (clone[0].Rank - 1 == clone[1].Rank
                    && clone[1].Rank - 1 == clone[2].Rank
                    && clone[2].Rank - 1 == clone[3].Rank
                    && clone[3].Rank - 1 == clone[4].Rank)
                {
                    return clone.Take(5).ToList();
                }

                clone.RemoveAt(0);
            }

            // Check for ace-low straight
            var match = new[] { Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five }
                            .Select(x => cards.FirstOrDefault(y => y.Rank == x))
                            .Where(x => x != null)
                            .ToList();

            if (match.Count == 5)
                return match;

            return null;

        }
        public List<Card> GetStraightFlush(List<Card> cards)
        {
            var straight = GetStraight(cards);

            if(straight != null && GetFlush(straight) is {} straightFlush)
            {
                return straightFlush;
            }

            return null;
        }
        public List<Card> GetThreeOfAKind(List<Card> cards)
        {
            SanityCheck(cards);
            var groups = cards.GroupBy(x => x.Rank)
                              .Where(x => x.Count() >= 3)
                              .OrderByDescending(x => x.Key)
                              .ToList();

            if(groups.Any())
            {
                return groups[0].Take(3).ToList();
            }

            return null;

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

        public List<Card> GetFullHouse(List<Card> cards)
        {
            SanityCheck(cards);

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
