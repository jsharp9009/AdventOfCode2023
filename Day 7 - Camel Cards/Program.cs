using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CamelCards;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var hands = ParseInput(input);
        var result = SolvePart1(hands);
        Console.WriteLine("Part 1: " + result);
        result = SolvePart2(hands);
        Console.WriteLine("Part 2: " + result);
    }

    public static long SolvePart1(List<Hand> hands){
        var ordered = hands.Order(new HandComparer()).ToArray();
        var result = 0L;
        for(int i = 0; i < ordered.Count(); i++){
            result += ordered[i].Bet * (i + 1);
        }
        return result;
    }

    public static long SolvePart2(List<Hand> hands){
        hands.ForEach(h => h.JokersWild = true);
        return SolvePart1(hands);
    }

    static List<Hand> ParseInput(string[] input){
        var reg = new Regex("(.+) ([0-9]+)");
        List<Hand> ret = new List<Hand>();
        foreach(var line in input){
            var match = reg.Match(line);
            ret.Add(new Hand(match.Groups[1].ToString(), int.Parse(match.Groups[2].ToString())));
        }
        return ret;
    }
}
