using Cards.Models;
using Cards.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
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


                var simulators = Enumerable.Range(0, 32)
                                    .Select(x => new HandSimulator(hand, (byte)x, iterations))
                                    .ToList();

                foreach (var simulator in simulators)
                {
                    Console.WriteLine($"[{simulator.HeldPositions}]: {Stringify(simulator.HeldCards)}...");
                    simulator.Process();
                }

                var mostPoints = simulators.OrderByDescending(x => x.Score).First();
                Console.WriteLine($"Most Points: {Stringify(mostPoints.HeldCards)} ({mostPoints.Score} pts, {mostPoints.Wins} wins, {mostPoints.Wins * 1.0 / iterations:P2})");

                var mostWins = simulators.OrderByDescending(x => x.Wins).First();
                Console.WriteLine($"Most Wins: {Stringify(mostWins.HeldCards)} ({mostWins.Score} pts, {mostWins.Wins} wins, {mostWins.Wins * 1.0 / iterations:P2})");

                while (true)
                {
                    Console.Write("Run another? [Y, N (0-32)]: ");
                    var response = Console.ReadLine();

                    if (int.TryParse(response, out var value) && value >= 0 && value < 32)
                    {
                        var chosen = simulators.Single(x => x.HeldPositions == value);
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

    public class HandSimulator
    {
        public List<Card> Hand { get; private set; }
        public List<Card> HeldCards { get; private set; } = new List<Card>();
        public byte HeldPositions { get; private set; }
        public int Score { get; private set; }
        public int Wins { get; private set; }
        public Dictionary<HandType, int> HandTypeCount { get; private set; } = new Dictionary<HandType, int>();
        public int Iterations { get; private set; }

        public HandSimulator(List<Card> hand, byte heldPositions, int iterations)
        {
            Hand = hand;
            HeldPositions = heldPositions;
            Iterations = iterations;

            if ((HeldPositions & 1) == 1)
                HeldCards.Add(Hand[0]);

            if ((HeldPositions & 2) == 2)
                HeldCards.Add(Hand[1]);

            if ((HeldPositions & 4) == 4)
                HeldCards.Add(Hand[2]);

            if ((HeldPositions & 8) == 8)
                HeldCards.Add(Hand[3]);

            if ((HeldPositions & 16) == 16)
                HeldCards.Add(Hand[4]);
        }

        public void Process()
        { 

            var handService = new CardHandService();

            for(var i = 0; i < Iterations; i++)
            {
                var redeal = Shuffle(Hand).Take(5 - HeldCards.Count).ToList();
                redeal.AddRange(HeldCards);

                var result = handService.GetHand(redeal);

                var score = Score(result, 1);
                if(score > 0)
                {
                    Wins++;
                    Score += score;
                }

                HandTypeCount[result.HandType] = HandTypeCount.ContainsKey(result.HandType) ? HandTypeCount[result.HandType] + 1 : 1;

            }

        }
    }
}
