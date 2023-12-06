using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WaitForIt;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var races = ParseInput(input);
        SolvePart1(races);
        SolvePart2(races);
    }

    static void SolvePart1(List<Race> races) => Console.WriteLine("Part 1: " + races.Select(a => a.GetWinningTimesCount()).Aggregate((a, b) => a * b));

    static void SolvePart2(List<Race> races){
        var finalTime = new StringBuilder();
        var finalDistance = new StringBuilder();

        foreach(var race in races){
            finalTime.Append(race.Time.ToString());
            finalDistance.Append(race.Distance.ToString());
        }

        var finalRace = new Race(long.Parse(finalTime.ToString()), long.Parse(finalDistance.ToString()));

        Console.WriteLine("Part 2: " + finalRace.GetWinningTimesCount());
    }

    static List<Race> ParseInput(string[] input){
        var reg = new Regex("[0-9]+");
        var timeMatcher = reg.Matches(input[0]);
        var distMatcher = reg.Matches(input[1]);
        List<Race> result = new List<Race>();

        for(int i = 0; i < timeMatcher.Count; i++){
            var timeMatch = timeMatcher[i];
            var distMatch = distMatcher[i];

            result.Add(new Race(int.Parse(timeMatch.Value), int.Parse(distMatch.Value)));
        }
        return result;
    }
}

record Race(long Time, long Distance){
    public long GetWinningTimesCount(){
        long count = 0;
        for(long i = 1; i < Time; i++){
            var result = (Time - i) * i;
            if (result > Distance) count++;
        }
        return count;
    }
};
