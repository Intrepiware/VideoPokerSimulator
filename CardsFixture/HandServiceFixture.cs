using Cards.Models;
using Cards.Services.Impl;
using NUnit.Framework;
using System;
using System.Linq;

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
            [TestCase("Too few cards", "4-C", "3-D", "2-H", "A-S")]
            [TestCase("Too many cards", "4-C", "3-D", "2-H", "A-S", "K-C", "Q-D", "J-H", "10-S")]
            [TestCase("Duplicate cards", "A-H", "A-H", "5-S", "7-D", "10-C")]
            public void Will_Sanity_Check(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);

                Assert.Throws(typeof(ArgumentException), () => handService.GetPair(hand), $"Exception case \"{description}\" did not return an expected error");
            }

            [Test]
            public void Will_Confirm_Pair()
            {
                var cards = Card.FromString("A-H", "A-D", "5-S", "7-D", "10-C");

                var hand = handService.GetPair(cards);

                Assert.IsNotNull(hand);
                CollectionAssert.AreEqual(Card.FromString("A-H", "A-D"), hand);
            }

            [Test]
            [TestCase("Regular Pair", "5-S", "5-D", "10-C", "A-H", "A-D")]
            [TestCase("Three of a Kind", "5-S", "K-D", "A-C", "A-H", "A-D")]
            [TestCase("Full House", "5-S", "5-D", "A-C", "A-H", "A-D")]
            [TestCase("Full House 2", "5-S", "5-D", "5-C", "A-H", "A-D")]
            [TestCase("Four of a Kind", "5-S", "A-S", "A-C", "A-H", "A-D")]
            [TestCase("Seven Cards", "2-S", "3-D", "4-C", "5-H", "6-S", "A-H", "A-D")]
            public void Will_Confirm_Highest_Pair(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);

                var pair = handService.GetPair(hand);

                Assert.AreEqual(2, pair.Count, $"Did not find pair in {description}");
                Assert.IsTrue(pair.TrueForAll(x => x.Rank == Rank.Ace), $"Did not find highest rank in {description}");

            }

            [Test]
            public void Will_Reject_Non_Pair()
            {
                var cards = Card.FromString("5-S", "8-D", "10-C", "A-H", "J-D");

                Assert.IsNull(handService.GetPair(cards));
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
            [TestCase("Regular Two Pair", "5-S", "5-D", "10-C", "A-H", "A-D")]
            [TestCase("Full House", "5-D", "A-D", "A-C", "5-H", "5-C")]
            [TestCase("Full House 2", "5-D", "A-D", "A-C", "A-H", "5-H")]
            [TestCase("Seven Cards", "5-S", "5-D", "10-C", "A-H", "A-D", "2-C", "4-D")]
            public void Will_Confirm_Two_Pair(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);

                var twoPair = handService.GetTwoPair(hand);
                Assert.IsNotNull(twoPair, $"Could not find two pair in {description}");

                Assert.AreEqual(4, twoPair.Count);
                Assert.AreEqual(2, twoPair.Where(x => x.Rank == Rank.Ace).Count());
                Assert.AreEqual(2, twoPair.Where(x => x.Rank == Rank.Five).Count());
            }

            [Test]
            [TestCase("Single Pair", "A-H", "A-D", "5-S", "7-D", "10-C")]
            [TestCase("Straight", "2-C", "3-H", "4-D", "5-S", "6-C")]
            [TestCase("Seven Card Straight", "2-C", "3-H", "4-D", "5-S", "6-C", "7-H", "8-D")]
            public void Will_Reject_Non_Two_Pair(string description, params string[] cards)
            {
                var hand = Card.FromString(cards);

                Assert.IsNull(handService.GetTwoPair(hand), $"Incorrectly got two pair from {description}");
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

                var hand = handService.GetThreeOfAKind(cards);

                Assert.AreEqual(Rank.Seven, hand[0].Rank);
            }

            [Test]
            public void Will_Confirm_Four_of_A_Kind()
            {
                var cards = Card.FromString("A-H", "2-C", "A-C", "A-S", "A-D");

                var hand = handService.GetThreeOfAKind(cards);

                Assert.AreEqual(Rank.Ace, hand[0].Rank);
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsNull(handService.GetThreeOfAKind(cards));
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

                var result = handService.GetFourOfAKind(cards);

                Assert.IsTrue(result.All(x => x.Rank == Rank.Three));
            }

            [Test]
            public void Will_Reject_Pair()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "9-H");

                Assert.IsNull(handService.GetFourOfAKind(cards));
            }

            [Test]
            public void Will_Reject_Three_of_A_Kind()
            {
                var cards = Card.FromString("7-S", "7-C", "2-C", "A-D", "7-H");

                Assert.IsNull(handService.GetFourOfAKind(cards));
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
            [TestCase("Five card hand", "3-C", "6-C", "Q-C", "K-C", "7-C")]
            [TestCase("Seven card hand", "3-C", "6-C", "Q-C", "K-C", "7-C", "2-D", "9-H")]
            public void Will_Confirm_Flush(string scenarioName, params string[] args)
            {
                var cards = Card.FromString(args);

                Assert.IsNotNull(handService.GetFlush(cards), $"Did not recognize flush in {scenarioName}");
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsNotNull(handService.GetFlush(cards));
            }

            [Test]
            public void Will_Reject_Non_Flush()
            {
                var cards = Card.FromString("3-C", "6-C", "Q-C", "K-H", "7-C");

                Assert.IsNull(handService.GetFlush(cards));
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

                Assert.IsNotNull(handService.GetStraight(cards));
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight()
            {
                var cards = Card.FromString("A-C", "2-D", "3-S", "4-H", "5-C");

                Assert.IsNotNull(handService.GetStraight(cards));
            }

            [Test]
            // 5♦ 4♠ 2♣ 4♣ 3♣
            [TestCase("4-C", "6-S", "3-H", "7-H", "10-D")]
            [TestCase("5-D", "4-S", "2-C", "4-C", "3-C")]
            public void Will_Reject_Non_Straight(string card1, string card2, string card3, string card4, string card5)
            {
                var cards = Card.FromString(card1, card2, card3, card4, card5);

                Assert.IsNull(handService.GetStraight(cards));
            }

            [Test]
            [TestCase("Ace Low", Rank.Ace, "A-C", "2-D", "3-H", "4-S", "5-C", "10-D", "J-H")]
            [TestCase("Bottom of Hand", Rank.Six, "2-D", "3-H", "4-S", "5-C", "6-D", "J-H", "Q-S")]
            [TestCase("Middle of Hand", Rank.Eight, "2-D", "4-S", "5-C", "6-D", "7-D", "8-H", "Q-S")]
            [TestCase("Top of Hand", Rank.Queen, "2-D", "4-S", "8-C", "9-D", "10-D", "J-H", "Q-S")]
            [TestCase("Embedded Pair", Rank.Six, "2-D", "3-H", "3-D", "4-S", "5-C", "6-D", "J-H")]
            [TestCase("Two Embedded Pair", Rank.Six, "2-D", "3-H", "3-D", "4-S", "4-C", "5-C", "6-D")]
            public void Will_Confirm_In_Seven_Card_Hand(string scenarioName, Rank expectedRank, params string[] args)
            {
                var cards = Card.FromString(args);

                var result = handService.GetStraight(cards);
                
                Assert.IsNotNull(result, $"Did not recognize \"{scenarioName}\" scenario");

                Assert.AreEqual(expectedRank, result.Max(x => x.Rank), $"Did not return expected hand with top rank {expectedRank}");
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

                Assert.IsNull(handService.GetStraightFlush(cards));
            }

            [Test]
            public void Will_Reject_Straight()
            {
                var cards = Card.FromString("4-C", "6-S", "3-H", "7-H", "5-D");

                Assert.IsNull(handService.GetStraightFlush(cards));
            }

            [Test]
            public void Will_Confirm_Straight_Flush()
            {
                var cards = Card.FromString("3-C", "4-C", "5-C", "6-C", "7-C");

                Assert.IsNotNull(handService.GetStraightFlush(cards));
            }

            [Test]
            public void Will_Confirm_Ace_Low_Straight_Flush()
            {
                var cards = Card.FromString("A-C", "2-C", "3-C", "4-C", "5-C");

                Assert.IsNotNull(handService.GetStraightFlush(cards));                
            }

            [Test]
            public void Will_Reject_Non_Straight_Flush()
            {
                var cards = Card.FromString("6-S", "5-C", "A-H", "5-H", "4-H");

                Assert.IsNull(handService.GetStraightFlush(cards));
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

                Assert.IsNotNull(handService.GetFullHouse(hand), $"Did not confirm {description}");
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
                Assert.IsNull(handService.GetFullHouse(hand), $"Failed to reject {description}");

            }

            [Test]
            public void Will_Confirm_FullHouse_Ranks_And_Cards()
            {
                var cards = Card.FromString("2-S", "2-H", "5-S", "2-D", "5-H");

                var hand = handService.GetFullHouse(cards);

                Assert.IsNotNull(hand);
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

            // [Test]
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
            public void Will_Recognize_Ace_Low_Straight()
            {
                var cards = Card.FromString("A-C", "2-S", "3-H", "4-H", "5-D");

                var hand = handService.GetHand(cards);

                Assert.AreEqual(HandType.Straight, hand.HandType);
                Assert.AreEqual(Rank.Five, hand.PrimaryRank);
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

            // [Test]
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
