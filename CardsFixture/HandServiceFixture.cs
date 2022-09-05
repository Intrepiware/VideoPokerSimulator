using Cards.Models;
using Cards.Services.Impl;
using NUnit.Framework;

namespace CardsFixture
{

    public class HandServiceFixture
    {
        [TestFixture]
        public class When_Checking_Pairs
        {
            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Pair()
            {
                var cards = Card.FromString("A-H", "A-D", "5-S", "7-D", "10-C");

                Assert.IsTrue(handService.IsPair(cards, out var highestPair));

                Assert.AreEqual(Rank.Ace, highestPair);
            }

            [Test]
            public void Will_Confirm_Highest_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D" );

                Assert.IsTrue(handService.IsPair(cards, out var highestPair));

                Assert.AreEqual(Rank.Ace, highestPair);

            }

            [Test]
            public void Will_Reject_Non_Pair()
            {
                var cards = Card.FromString("5-S", "8-D", "10-C", "A-H", "J-D");

                Assert.IsFalse(handService.IsPair(cards, out var highestPair));

                Assert.IsNull(highestPair);
            }
        }

        [TestFixture]
        public class When_Checking_Two_Pairs
        {

            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Two_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D");

                Assert.IsTrue(handService.IsTwoPair(cards, out var highPair, out var lowPair));

                Assert.AreEqual(Rank.Ace, highPair);
                Assert.AreEqual(Rank.Five, lowPair);
            }

            [Test]
            public void Will_Confirm_Full_House()
            {
                var cards = Card.FromString("9-D", "2-D", "2-C", "2-H", "9-H");

                Assert.IsTrue(handService.IsTwoPair(cards, out var highPair, out var lowPair));

                Assert.AreEqual(Rank.Nine, highPair);
                Assert.AreEqual(Rank.Two, lowPair);
            }

            [Test]
            public void Will_Reject_Single_Pair()
            {
                var cards = Card.FromString("A-H", "A-D", "5-S", "7-D", "10-C");

                Assert.False(handService.IsTwoPair(cards, out var rank1, out var rank2));

                Assert.IsNull(rank1);
                Assert.IsNull(rank2);

            }
        }

        [TestFixture]
        public class When_Checking_Three_Of_A_Kind
        {

            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsTrue(handService.IsThreeOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Four_of_A_Kind()
            {
                var cards = Card.FromString("A-H", "2-C", "A-C", "A-S", "A-D");

                Assert.IsTrue(handService.IsThreeOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Ace, rank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsFalse(handService.IsThreeOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }

        }

        [TestFixture]
        public class When_Checking_Four_Of_A_Kind
        {
            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Four_Of_A_Kind()
            {
                var cards = Card.FromString("5-C", "3-H", "3-D", "3-S", "3-C");

                Assert.IsTrue(handService.IsFourOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Three, rank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsFalse(handService.IsFourOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Reject_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsFalse(handService.IsFourOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_Flush
        {
            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-C", "7-C");

                Assert.IsTrue(handService.IsFlush(cards));
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsTrue(handService.IsFlush(cards));
            }

            [Test]
            public void Will_Reject_Non_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-H", "7-C");

                Assert.IsFalse(handService.IsFlush(cards));
            }

        }

        [TestFixture]
        public class When_Checking_Straight
        {
            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                Assert.IsTrue(handService.IsStraight(cards, out var rank));

                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight()
            {
                var cards = Card.FromString("A-C", "2-D", "3-S", "4-H", "5-C");

                Assert.IsTrue(handService.IsStraight(cards, out var rank));
                Assert.AreEqual(Rank.Five, rank);
            }

            [Test]
            public void Will_Confirm_Non_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "10-D");

                Assert.IsFalse(handService.IsStraight(cards, out var rank));

                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_Straight_Flush
        {

            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Reject_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-C", "7-C");

                Assert.IsFalse(handService.IsStraightFlush(cards, out var rank));
                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Reject_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                Assert.IsFalse(handService.IsStraightFlush(cards, out var rank));

                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsTrue(handService.IsStraightFlush(cards, out var rank));
                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight_Flush()
            {
                var cards = Card.FromString("A-C", "2-C", "3-C", "4-C", "5-C");

                Assert.IsTrue(handService.IsStraightFlush(cards, out var rank));
                Assert.AreEqual(Rank.Five, rank);
            }

            [Test]
            public void Will_Reject_Non_Straight_Flush()
            {
                var cards = Card.FromString("6-S", "5-C", "A-H", "5-H", "4-H");

                Assert.IsFalse(handService.IsStraightFlush(cards, out var rank));
                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_FullHouse
        {

            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Confirm_FullHouse()
            {
                var cards = Card.FromString("2-S", "2-H", "5-S", "2-D", "5-H");

                Assert.IsTrue(handService.IsFullHouse(cards, out var threeOfAKindRank, out var pairRank));

                Assert.AreEqual(Rank.Two, threeOfAKindRank);
                Assert.AreEqual(Rank.Five, pairRank);
            }

            [Test]
            public void Will_Reject_Two_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D");

                Assert.IsFalse(handService.IsFullHouse(cards, out var threeOfAKindRank, out var pairRank));

                Assert.IsNull(threeOfAKindRank);
                Assert.IsNull(pairRank);
            }

            [Test]
            public void Will_Reject_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsFalse(handService.IsFullHouse(cards, out var threeOfAKindRank, out var pairRank));

                Assert.IsNull(threeOfAKindRank);
                Assert.IsNull(pairRank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsFalse(handService.IsFullHouse(cards, out var threeOfAKindRank, out var pairRank));

                Assert.IsNull(threeOfAKindRank);
                Assert.IsNull(pairRank);
            }
        }

        [TestFixture]
        public class When_Evaluating_Hand
        {

            ICardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new ICardHandService();
            }

            [Test]
            public void Will_Recognize_HighCard()
            {
                var cards = Card.FromString("8-D", "3-D", "9-S", "5-S", "A-D");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.HighCard, hand.HandType);
                Assert.AreEqual(Rank.Ace, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);

            }

            [Test]
            public void Will_Recognize_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.Pair, hand.HandType);
                Assert.AreEqual(Rank.Seven, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);
            }

            [Test]
            public void Will_Recognize_Two_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.TwoPair, hand.HandType);
                Assert.AreEqual(Rank.Ace, hand.PrimaryRank);
                Assert.AreEqual(Rank.Five, hand.SecondaryRank);
            }


            [Test]
            public void Will_Recognize_Three_Of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.ThreeOfAKind, hand.HandType);
                Assert.AreEqual(Rank.Seven, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);
            }

            [Test]
            public void Will_Recognize_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.Straight, hand.HandType);
                Assert.AreEqual(Rank.Seven, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);
            }

            [Test]
            public void Will_Recognize_Flush()
            {
                var cards = Card.FromString("8-D", "3-D", "9-D", "5-D", "A-D");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.Flush, hand.HandType);
                Assert.AreEqual(Rank.Ace, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);
            }

            [Test]
            public void Will_Recognize_Full_House()
            {
                var cards = Card.FromString("2-S", "2-H", "5-S", "2-D", "5-H");
                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.FullHouse, hand.HandType);
                Assert.AreEqual(Rank.Two, hand.PrimaryRank);
                Assert.AreEqual(Rank.Five, hand.SecondaryRank);
            }

            [Test]
            public void Will_Recognize_Straight_Flush()
            {
                var cards = Card.FromString("4-C", "6-C", "3-C", "7-C", "5-C");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.StraightFlush, hand.HandType);
                Assert.AreEqual(Rank.Seven, hand.PrimaryRank);
                Assert.IsNull(hand.SecondaryRank);
            }
        }
    }
}
