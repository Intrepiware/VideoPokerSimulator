using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Models
{
    public class Card
    {
        public Rank Rank { get; private set; }
        public Suit Suit { get; private set; }


        public static Card FromInt(int num)
        {
            if (num < 0 || num > 51)
                throw new ArgumentException("num must be between 0 and 51");

            return new Card(
                (Rank)(num % 13),
                (Suit)(num / 13)
            );
        }

        public static List<Card> FromString(params string[] args)
        {
            return args.Select(Parse).ToList();
        }

        private static Card Parse(string card)
        {
            if ((card ?? string.Empty).Split('-').Length != 2)
                throw new ArgumentException($"{card} must contain exactly one \"-\".");
            var parts = card.Split('-');

            Rank rank = default;
            Suit suit = default;

            switch(parts[0])
            {
                case "A": rank = Rank.Ace; break;
                case "J": rank = Rank.Jack; break;
                case "Q": rank = Rank.Queen; break;
                case "K": rank = Rank.King; break;
                default:
                    var num = byte.Parse(parts[0]);
                    if (num <= 0 || num > 10)
                        throw new ArgumentException($"{card}: {parts[0]} is not a valid rank");
                    rank = (Rank)(num - 2);
                    break;
            }

            switch(parts[1])
            {
                case "H": suit = Suit.Hearts; break;
                case "C": suit = Suit.Clubs; break;
                case "S": suit = Suit.Spades; break;
                case "D": suit = Suit.Diamonds; break;
                default:
                    throw new ArgumentException($"{card}: {parts[1]} is not a valid suit");
            }

            return new Card(rank, suit);

        }


        public void test()
        {
            
        }

        public Card (Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }
    }

    public enum Rank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
}
