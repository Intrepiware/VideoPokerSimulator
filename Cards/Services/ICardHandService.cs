using Cards.Models;
using System.Collections.Generic;

namespace Cards.Services
{
    public interface ICardHandService
    {
        Hand GetHand(List<Card> cards);

        bool GetPair(List<Card> cards, out Rank? highestPair);
        bool GetTwoPair(List<Card> cards, out Rank? highestPair, out Rank? lowestPair);
        bool GetThreeOfAKind(List<Card> cards, out Rank? rank);
        bool GetStraight(List<Card> cards, out Rank? rank);
        bool GetFlush(List<Card> cards);
        bool GetFourOfAKind(List<Card> cards, out Rank? rank);
        bool GetStraightFlush(List<Card> cards, out Rank? rank);
        List<Card> GetFullHouse(List<Card> cards, out Rank? threeOfAKindRank, out Rank? pairRank);

        byte Compare(Hand hand1, Hand hand2);
    }
}
