using System;
using System.Collections.Generic;
using System.Text;

namespace Cards.Models
{

    public class Hand
    {
        public HandType HandType { get; set; }
        public Rank PrimaryRank { get; set; }
        public Rank? SecondaryRank { get; set; }
    }

    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }
}
