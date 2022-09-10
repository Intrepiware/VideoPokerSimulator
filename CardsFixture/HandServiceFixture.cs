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
            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Pair()
            {
                var cards = Card.FromString("A-H", "A-D", "5-S", "7-D", "10-C");

                Assert.IsTrue(handService.GetPair(cards, out var highestPair));

                Assert.AreEqual(Rank.Ace, highestPair);
            }

            [Test]
            public void Will_Confirm_Highest_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D" );

                Assert.IsTrue(handService.GetPair(cards, out var highestPair));

                Assert.AreEqual(Rank.Ace, highestPair);

            }

            [Test]
            public void Will_Reject_Non_Pair()
            {
                var cards = Card.FromString("5-S", "8-D", "10-C", "A-H", "J-D");

                Assert.IsFalse(handService.GetPair(cards, out var highestPair));

                Assert.IsNull(highestPair);
            }
        }

        [TestFixture]
        public class When_Checking_Two_Pairs
        {

            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Two_Pair()
            {
                var cards = Card.FromString("5-S", "5-D", "10-C", "A-H", "A-D");

                Assert.IsTrue(handService.GetTwoPair(cards, out var highPair, out var lowPair));

                Assert.AreEqual(Rank.Ace, highPair);
                Assert.AreEqual(Rank.Five, lowPair);
            }

            [Test]
            public void Will_Confirm_Full_House()
            {
                var cards = Card.FromString("9-D", "2-D", "2-C", "2-H", "9-H");

                Assert.IsTrue(handService.GetTwoPair(cards, out var highPair, out var lowPair));

                Assert.AreEqual(Rank.Nine, highPair);
                Assert.AreEqual(Rank.Two, lowPair);
            }

            [Test]
            public void Will_Reject_Single_Pair()
            {
                var cards = Card.FromString("A-H", "A-D", "5-S", "7-D", "10-C");

                Assert.False(handService.GetTwoPair(cards, out var rank1, out var rank2));

                Assert.IsNull(rank1);
                Assert.IsNull(rank2);

            }
        }

        [TestFixture]
        public class When_Checking_Three_Of_A_Kind
        {

            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsTrue(handService.GetThreeOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Four_of_A_Kind()
            {
                var cards = Card.FromString("A-H", "2-C", "A-C", "A-S", "A-D");

                Assert.IsTrue(handService.GetThreeOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Ace, rank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsFalse(handService.GetThreeOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }

        }

        [TestFixture]
        public class When_Checking_Four_Of_A_Kind
        {
            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Four_Of_A_Kind()
            {
                var cards = Card.FromString("5-C", "3-H", "3-D", "3-S", "3-C");

                Assert.IsTrue(handService.GetFourOfAKind(cards, out var rank));

                Assert.AreEqual(Rank.Three, rank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsFalse(handService.GetFourOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Reject_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsFalse(handService.GetFourOfAKind(cards, out var rank));

                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_Flush
        {
            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-C", "7-C");

                Assert.IsTrue(handService.GetFlush(cards));
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsTrue(handService.GetFlush(cards));
            }

            [Test]
            public void Will_Reject_Non_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-H", "7-C");

                Assert.IsFalse(handService.GetFlush(cards));
            }

        }

        [TestFixture]
        public class When_Checking_Straight
        {
            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Confirm_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                Assert.IsTrue(handService.GetStraight(cards, out var rank));

                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight()
            {
                var cards = Card.FromString("A-C", "2-D", "3-S", "4-H", "5-C");

                Assert.IsTrue(handService.GetStraight(cards, out var rank));
                Assert.AreEqual(Rank.Five, rank);
            }

            [Test]
            // 5♦ 4♠ 2♣ 4♣ 3♣
            [TestCase("4-C", "6-S", "3-H", "7-H", "10-D")]
            [TestCase("5-D", "4-S", "2-C", "4-C", "3-C")]
            public void Will_Reject_Non_Straight(string card1, string card2, string card3, string card4, string card5)
            {
                var cards = Card.FromString(card1, card2, card3, card4, card5);

                Assert.IsFalse(handService.GetStraight(cards, out var rank));

                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_Straight_Flush
        {

            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            public void Will_Reject_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-C", "7-C");

                Assert.IsFalse(handService.GetStraightFlush(cards, out var rank));
                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Reject_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                Assert.IsFalse(handService.GetStraightFlush(cards, out var rank));

                Assert.IsNull(rank);
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsTrue(handService.GetStraightFlush(cards, out var rank));
                Assert.AreEqual(Rank.Seven, rank);
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight_Flush()
            {
                var cards = Card.FromString("A-C", "2-C", "3-C", "4-C", "5-C");

                Assert.IsTrue(handService.GetStraightFlush(cards, out var rank));
                Assert.AreEqual(Rank.Five, rank);
            }

            [Test]
            public void Will_Reject_Non_Straight_Flush()
            {
                var cards = Card.FromString("6-S", "5-C", "A-H", "5-H", "4-H");

                Assert.IsFalse(handService.GetStraightFlush(cards, out var rank));
                Assert.IsNull(rank);
            }
        }

        [TestFixture]
        public class When_Checking_FullHouse
        {

            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
            }

            [Test]
            [TestCase("Standard Full House", "2-S", "2-H", "5-S", "2-D", "5-H")]
            [TestCase("Full House in Seven Cards", "2-S", "2-H", "5-S", "2-D", "5-H", "3-C", "9-D")]
            [TestCase("Full House in Seven Cards with Four-of-a-Kind", "2-S", "2-H", "5-S", "2-D", "5-H", "2-C", "9-D")]
            public void Will_Confirm_FullHouse(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);

                Assert.IsNotNull(handService.GetFullHouse(hand, out var _, out var _), $"Did not confirm {description}");
            }

            [Test]
            [TestCase("Two Pair", "5-S", "5-D", "10-C", "A-H", "A-D")]
            [TestCase("Three of a Kind", "7-S", "7-C", "2-C", "A-D", "7-H")]
            [TestCase("Three of a Kind in Five Cards", "7-S", "7-C", "7-D", "A-D", "7-H")]
            [TestCase("Pair", "7-S", "7-C", "2-C", "A-D", "9-H")]
            [TestCase("Straight", "2-D", "3-D", "4-D", "5-D", "6-D")]
            public void Will_Reject_NonFullHouse(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);
                Assert.IsNull(handService.GetFullHouse(hand, out _, out _), $"Failed to reject {description}");

            }

            [Test]
            public void Will_Confirm_FullHouse_Ranks_And_Cards()
            {
                var cards = Card.FromString("2-S", "2-H", "5-S", "2-D", "5-H");

                var hand = handService.GetFullHouse(cards, out var threeOfAKindRank, out var pairRank);

                Assert.IsNotNull(hand);
                Assert.AreEqual(Rank.Two, threeOfAKindRank);
                Assert.AreEqual(Rank.Five, pairRank);
                CollectionAssert.AreEquivalent(cards, hand);
            }
        }

        [TestFixture]
        public class When_Evaluating_Hand
        {

            CardHandService handService;

            [SetUp]
            public void Setup()
            {
                handService = new CardHandService();
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
            public void Will_Recognize_Flush_In_Ten_Cards()
            {
                var cards = Card.FromString("J-S", "2-D", "6-H", "7-H", "7-S", "Q-S", "6-S", "9-S", "3-D", "Q-C");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.Flush, hand.HandType);
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

            [Test]
            public void Will_Handle_Ten_Cards()
            {
                var cards = Card.FromString("5-S", "Q-C", "4-D", "J-C", "J-S", "3-H", "10-H", "K-S", "8-H", "3-C");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.TwoPair, hand.HandType);
                Assert.AreEqual(Rank.Jack, hand.PrimaryRank);
                Assert.AreEqual(Rank.Three, hand.SecondaryRank);
            }
        }
    }
}
