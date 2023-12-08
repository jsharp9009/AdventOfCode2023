using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HauntedWasteland;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var (enumerator, rules) = ParseInput(input);
        //SolvePart1(enumerator, rules);
        SolvePart2(enumerator, rules);
    }

    static void SolvePart1(DirectionEnumerator enumerator, Dictionary<string, string[]> rules)
    {
        var steps = GetSteps(enumerator, rules, "AAA", "ZZZ");
        Console.WriteLine("Part 1: " + steps);
    }

    static void SolvePart2(DirectionEnumerator enumerator, Dictionary<string, string[]> rules)
    {
        var starts = rules.Keys.Where(c => c[2] == 'A');
        var distances = new List<int>();
        foreach (var start in starts)
        {
            distances.Add(FindStepsToZ(enumerator, rules, start));
        }
        long lcm = distances.First();
        for(int i = 1; i < distances.Count; i++){
            lcm = LeastCommonMultiple(distances[i], lcm);
        }
        Console.WriteLine("Part 2: " + lcm);
    }

    static (DirectionEnumerator, Dictionary<string, string[]>) ParseInput(string[] input)
    {
        var enumerator = new DirectionEnumerator(input[0]);
        var rules = new Dictionary<string, string[]>();
        var reg = new Regex(@"([A-z0-9]+) = \(([A-z0-9]+)\, ([A-z0-9]+)\)");
        for (int i = 2; i < input.Length; i++)
        {
            var matches = reg.Match(input[i]);
            rules.Add(matches.Groups[1].Value, new string[] { matches.Groups[2].Value, matches.Groups[3].Value });
        }

        return new(enumerator, rules);
    }

    static int GetSteps(DirectionEnumerator enumerator, Dictionary<string, string[]> rules, string starting, string destination)
    {
        enumerator.Reset();
        var steps = 0;
        var currentLocation = starting;
        do
        {
            steps++;
            currentLocation = rules[currentLocation][enumerator.Current];
            if (currentLocation == destination) return steps;
        } while (enumerator.MoveNext());
        return steps;
    }

    static int FindStepsToZ(DirectionEnumerator enumerator, Dictionary<string, string[]> rules, string starting)
    {
        enumerator.Reset();
        var steps = 0;
        var currentLocation = starting;
        do
        {
            steps++;
            currentLocation = rules[currentLocation][enumerator.Current];
            if (currentLocation[2] == 'Z') return steps;
        } while (enumerator.MoveNext());
        return steps;
    }

    static long gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long LeastCommonMultiple(long a, long b)
    {
        return (a / gcf(a, b)) * b;
    }
}
