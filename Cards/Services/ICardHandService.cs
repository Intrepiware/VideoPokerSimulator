using Cards.Models;
using System.Collections.Generic;

namespace Cards.Services
{
    public interface ICardHandService
    {
        Hand GetHand(List<Card> cards);

        bool IsPair(List<Card> cards, out Rank? highestPair);
        bool IsTwoPair(List<Card> cards, out Rank? highestPair, out Rank? lowestPair);
        bool IsThreeOfAKind(List<Card> cards, out Rank? rank);
        bool IsStraight(List<Card> cards, out Rank? rank);
        bool IsFlush(List<Card> cards);
        bool IsFourOfAKind(List<Card> cards, out Rank? rank);
        bool IsStraightFlush(List<Card> cards, out Rank? rank);
        bool IsFullHouse(List<Card> cards, out Rank? threeOfAKindRank, out Rank? pairRank);

        byte Compare(Hand hand1, Hand hand2);
    }
}
