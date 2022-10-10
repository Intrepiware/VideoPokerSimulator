using Cards.Models;
using System.Collections.Generic;

namespace Cards.Services
{
    public interface ICardHandService
    {
        Hand GetHand(List<Card> cards);

        List<Card> GetPair(List<Card> cards);
        List<Card> GetTwoPair(List<Card> cards);
        List<Card> GetThreeOfAKind(List<Card> cards);
        List<Card> GetStraight(List<Card> cards);
        List<Card> GetFlush(List<Card> cards);
        List<Card> GetFourOfAKind(List<Card> cards);
        List<Card> GetStraightFlush(List<Card> cards);
        List<Card> GetFullHouse(List<Card> cards);

        byte Compare(Hand hand1, Hand hand2);
    }
}
