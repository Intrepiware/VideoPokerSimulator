using Cards.Models;
using Cards.Services.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Cards.Services.Impl.UtilService;

namespace Cards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int iterations = 50000;

            List<Card> hand = new List<Card>();
            var response = string.Empty;

            Console.Write("[I]nput hand, or [R]andom?");
            response = Console.ReadKey().KeyChar.ToString();

            while (true)
            {

                Console.Clear();

                if (response == "i")
                {
                    Console.Write("\nInput hand:");
                    var input = Console.ReadLine();
                    hand = Card.FromString(input.Split(' ')).Take(5).ToList();
                }
                else
                {
                    hand = Shuffle(null).Take(5).ToList();
                    Console.WriteLine($"Drew {Stringify(hand)}");
                    Console.WriteLine("Press [R] to Redeal, or any other key to confirm...");
                    if (Console.ReadKey().KeyChar.ToString().ToLower() == "r")
                        continue;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var heldPositions = Enumerable.Range(0, 32)
                                    .ToList();

                var threadResults = new ConcurrentBag<HandSimulatorResult>();
                int resultCount = 0;

                Console.Write($"Processing [{new string(' ', 32)}] (0%)");

                Parallel.ForEach(heldPositions,
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    heldPosition =>
                    {
                        var simulator = new HandSimulator();
                        threadResults.Add(simulator.Process(hand, (byte)heldPosition, iterations));

                        var results = Interlocked.Increment(ref resultCount);
                        var spaceCount = Math.Max(32 - results, 0);
                        var percent = Math.Min(results * 1.0 / 32, 1);
                        Console.CursorLeft = 0;
                        Console.Write($"Processing [{new string('.', results)}{new string(' ', spaceCount)}] ({percent:P0})");
                    });

                sw.Stop();
                
                Console.WriteLine($"\nAnalysis completed in {sw.Elapsed.TotalMilliseconds / 1000:N2} seconds.");
                var results = threadResults.OrderBy(x => x.HeldPositions).ToList();
                foreach (var result in results)
                    Console.WriteLine($"[{result.HeldPositions}]: {Stringify(result.HeldCards)}");


                var mostPoints = results.OrderByDescending(x => x.Score).First();
                Console.WriteLine($"Most Points: {Stringify(mostPoints.HeldCards)} ({mostPoints.Score:N0} pts, {mostPoints.Wins:N0} wins, {mostPoints.Wins * 1.0 / iterations:P2})");

                var mostWins = results.OrderByDescending(x => x.Wins).ThenByDescending(x => x.Score).First();
                Console.WriteLine($"Most Wins: {Stringify(mostWins.HeldCards)} ({mostWins.Score:N0} pts, {mostWins.Wins:N0} wins, {mostWins.Wins * 1.0 / iterations:P2})");

                while (true)
                {
                    Console.Write("[I]nput, [R]andom, [Q]uit or (0-31): ");
                    response = Console.ReadLine();

                    if (int.TryParse(response, out var value) && value >= 0 && value < 32)
                    {
                        var chosen = results.Single(x => x.HeldPositions == value);
                        Console.WriteLine($"{Stringify(chosen.HeldCards)}:");
                        Console.WriteLine($"{chosen.Score:N0} points, {chosen.Wins:N0} wins, {chosen.Wins * 1.0 / iterations:P2}");
                        var handTypes = chosen.HandTypeCount.OrderByDescending(x => x.Key);
                        foreach (var handType in handTypes)
                            Console.WriteLine($"{handType.Key}: {handType.Value:N0} wins ({handType.Value * 1.0 / iterations:P2})");

                        Console.WriteLine();
                    }
                    else if (response.ToLower() == "q")
                        return;
                    else
                        break;
                }
            }
        }

    }

    public class HandSimulatorResult
    {
        public List<Card> HeldCards { get; set; }
        public byte HeldPositions { get; set; }
        public int Score { get; set; }
        public int Wins { get; set; }
        public Dictionary<HandType, int> HandTypeCount { get; set; }
        public int Iterations { get; set; }
    }

    public class HandSimulator
    {

        public List<Card> GetHeldCards(List<Card> hand, byte heldPositions)
        {
            var heldCards = new List<Card>();

            if ((heldPositions & 1) == 1)
                heldCards.Add(hand[0]);

            if ((heldPositions & 2) == 2)
                heldCards.Add(hand[1]);

            if ((heldPositions & 4) == 4)
                heldCards.Add(hand[2]);

            if ((heldPositions & 8) == 8)
                heldCards.Add(hand[3]);

            if ((heldPositions & 16) == 16)
                heldCards.Add(hand[4]);

            return heldCards;
        }

        public HandSimulatorResult Process(List<Card> hand, byte heldPositions, int iterations)
        {
            var heldCards = GetHeldCards(hand, heldPositions);
            var handService = new CardHandService();

            if(heldCards.Count >= 4)
                return BruteForce(hand, heldCards, heldPositions, iterations, handService);

            int wins = 0, score = 0;
            var handTypeCount = new Dictionary<HandType, int>();
            for (var i = 0; i < iterations; i++)
            {
                var redeal = Shuffle(hand).Take(5 - heldCards.Count).ToList();
                redeal.AddRange(heldCards);

                var result = handService.GetHand(redeal);

                var handScore = Score(result, 1);
                if (handScore > 0)
                {
                    wins++;
                    score += handScore;
                }

                handTypeCount[result.HandType] = handTypeCount.ContainsKey(result.HandType) ? handTypeCount[result.HandType] + 1 : 1;

            }

            return new HandSimulatorResult
            {
                HeldCards = heldCards,
                HandTypeCount = handTypeCount,
                HeldPositions = heldPositions,
                Iterations = iterations,
                Score = score,
                Wins = wins
            };

        }

        private HandSimulatorResult BruteForce(List<Card> hand, List<Card> heldCards, byte heldPositions, int iterations, CardHandService handService)
        {
            if (heldCards.Count < 4)
                throw new NotImplementedException("BruteForce only supports holding 0 or 1 cards"); // ...for now

            if (heldCards.Count == 5)
            {
                var result = handService.GetHand(heldCards);
                var handScore = Score(result, 1);

                var output = new HandSimulatorResult
                {
                    Iterations = iterations,
                    HeldPositions = heldPositions,
                    HeldCards = heldCards,
                    Score = handScore * iterations,
                    Wins = (handScore > 0) ? iterations : 0,
                    HandTypeCount = new Dictionary<HandType, int> { [result.HandType] = iterations }
                };

                return output;
            }
            else
            {
                decimal wins = 0, score = 0;
                var handTypeCount = new Dictionary<HandType, int>();
                var undrawnCards = Enumerable.Range(0, 52).Select(x => Card.FromInt(x))
                                            .Where(x => !hand.Any(y => x.Rank == y.Rank && x.Suit == y.Suit))
                                            .ToList();

                foreach (var card in undrawnCards)
                {
                    var testHand = heldCards.Append(card).ToList();
                    var result = handService.GetHand(testHand);
                    var handScore = Score(result, 1);
                    if (handScore > 0)
                    {
                        wins++;
                        score += handScore;
                    }
                    handTypeCount[result.HandType] = handTypeCount.ContainsKey(result.HandType) ? handTypeCount[result.HandType] + 1 : 1;
                }

                // Extrapolate the results
                wins = Math.Round(wins / undrawnCards.Count * iterations, 0, MidpointRounding.ToEven);
                score = Math.Round(score / undrawnCards.Count * iterations, 0, MidpointRounding.ToEven);
                foreach (var keyValue in handTypeCount.ToList())
                {
                    var value = Convert.ToDecimal(keyValue.Value);
                    value = Math.Round(value / undrawnCards.Count * iterations, 0, MidpointRounding.ToEven);
                    handTypeCount[keyValue.Key] = Convert.ToInt32(value);
                }

                return new HandSimulatorResult
                {
                    HandTypeCount = handTypeCount,
                    HeldCards = heldCards,
                    HeldPositions = heldPositions,
                    Iterations = iterations,
                    Score = Convert.ToInt32(score),
                    Wins = Convert.ToInt32(wins)
                };

            }
        }
    }
}
