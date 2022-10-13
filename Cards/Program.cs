using Cards.Models;
using Cards.Services.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cards.Services.Impl.UtilService;

namespace Cards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int iterations = 50000;
            while (true)
            {

                var hand = Shuffle(null).Take(5).ToList();
                Console.WriteLine($"Running simulations on {Stringify(hand)}");

                Console.WriteLine("Press a key to continue...");
                Console.ReadKey();
                Console.WriteLine("Processing...");

                var heldPositions = Enumerable.Range(0, 32)
                                    .ToList();

                var threadResults = new ConcurrentBag<HandSimulatorResult>();

                Parallel.ForEach(heldPositions,
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    heldPosition =>
                    {
                        var simulator = new HandSimulator();
                        threadResults.Add(simulator.Process(hand, (byte)heldPosition, iterations));
                    });

                var results = threadResults.OrderBy(x => x.HeldPositions).ToList();

                foreach (var result in results)
                    Console.WriteLine($"[{result.HeldPositions}]: {Stringify(result.HeldCards)}");


                var mostPoints = results.OrderByDescending(x => x.Score).First();
                Console.WriteLine($"Most Points: {Stringify(mostPoints.HeldCards)} ({mostPoints.Score} pts, {mostPoints.Wins} wins, {mostPoints.Wins * 1.0 / iterations:P2})");

                var mostWins = results.OrderByDescending(x => x.Wins).ThenByDescending(x => x.Score).First();
                Console.WriteLine($"Most Wins: {Stringify(mostWins.HeldCards)} ({mostWins.Score} pts, {mostWins.Wins} wins, {mostWins.Wins * 1.0 / iterations:P2})");

                while (true)
                {
                    Console.Write("Run another? [Y, N (0-32)]: ");
                    var response = Console.ReadLine();

                    if (int.TryParse(response, out var value) && value >= 0 && value < 32)
                    {
                        var chosen = results.Single(x => x.HeldPositions == value);
                        Console.WriteLine($"{Stringify(chosen.HeldCards)}:");
                        Console.WriteLine($"{chosen.Score} points, {chosen.Wins} wins, {chosen.Wins * 1.0 / iterations:P2}");
                        var handTypes = chosen.HandTypeCount.OrderByDescending(x => x.Key);
                        foreach (var handType in handTypes)
                            Console.WriteLine($"{handType.Key}: {handType.Value} wins");

                        Console.WriteLine();
                    }
                    else if (response.ToLower() == "y")
                        break;
                    else
                        return;
                }

                Console.Clear();
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
            int wins = 0, score = 0;
            var handTypeCount = new Dictionary<HandType, int>();

            var heldCards = GetHeldCards(hand, heldPositions);

            var handService = new CardHandService();

            for (var i = 0; i < iterations; i++)
            {
                var redeal = Shuffle(hand).Take(5 - heldCards.Count).ToList();
                redeal.AddRange(heldCards);

                var result = handService.GetHand(redeal);

                var handScore = Score(result, 1);
                if(handScore > 0)
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
    }
}
