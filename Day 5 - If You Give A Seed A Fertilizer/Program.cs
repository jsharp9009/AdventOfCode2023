using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fertilizer;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var alm = ParseInput(input);

        SolvePart1(alm);
        SolvePart2(alm);
    }

    static void SolvePart1(Almanac alm)
    {
        var minLocation = long.MaxValue;
        foreach (var seed in alm.Seeds)
        {
            var location = GetLocation(seed, alm);
            if (location < minLocation) minLocation = location;
        }
        Console.WriteLine($"Part 1: {minLocation}");
    }

    static void SolvePart2(Almanac alm)
    {
        var minLocation = long.MaxValue;
        var combinedSeeds = new List<Range>();
        for(int i = 0; i < alm.Seeds.Count; i += 2){
            combinedSeeds.Add(new Range(alm.Seeds[i], alm.Seeds[i] + alm.Seeds[i + 1] - 1));
        }
        List<Range> seeds = combinedSeeds.OrderBy(s => s.start).Combine().ToList();

        foreach(var seed in seeds){
            var potentialSeeds = GetLocationRange(seed, alm);
            var min = potentialSeeds.Min(s => s.start);
            if (min < minLocation) minLocation = min;
        }
        Console.WriteLine($"Part 2: {minLocation}");
    }

    static Almanac ParseInput(string[] input)
    {
        var alm = new Almanac();
        alm.Seeds = ParseSeeds(input[0]);

        long seedStart = input.indexOf("seed-to-soil map:", 0);
        long seedEnd = input.indexOf("", seedStart);
        alm.SeedToSoil = ParseRuleset(input.SubArray(seedStart + 1, seedEnd - seedStart - 1));

        long soilStart = input.indexOf("soil-to-fertilizer map:", seedEnd);
        long soilEnd = input.indexOf("", soilStart);
        alm.SoilToFertilizer = ParseRuleset(input.SubArray(soilStart + 1, soilEnd - soilStart - 1));

        long fertStart = input.indexOf("fertilizer-to-water map:", soilEnd);
        long fertEnd = input.indexOf("", fertStart);
        alm.FertilizerToWater = ParseRuleset(input.SubArray(fertStart + 1, fertEnd - fertStart - 1));

        long waterStart = input.indexOf("water-to-light map:", fertEnd);
        long waterEnd = input.indexOf("", waterStart);
        alm.WaterToLight = ParseRuleset(input.SubArray(waterStart + 1, waterEnd - waterStart - 1));

        long lightStart = input.indexOf("light-to-temperature map:", waterEnd);
        long lightEnd = input.indexOf("", lightStart);
        alm.LightToTemp = ParseRuleset(input.SubArray(lightStart + 1, lightEnd - lightStart - 1));

        long tempStart = input.indexOf("temperature-to-humidity map:", lightEnd);
        long tempEnd = input.indexOf("", tempStart);
        alm.TempToHumidity = ParseRuleset(input.SubArray(tempStart + 1, tempEnd - tempStart - 1));

        long humStart = input.indexOf("humidity-to-location map:", tempEnd);
        alm.HumidityToLocation = ParseRuleset(input.SubArray(humStart + 1, input.Length - humStart - 1));

        return alm;
    }

    static List<long> ParseSeeds(string line)
    {
        var reg = new Regex("[0-9]+");
        var matcher = reg.Matches(line);
        List<long> seeds = new List<long>();
        for (int i = 0; i < matcher.Count; i++)
        {
            seeds.Add(long.Parse(matcher[i].Value));
        }
        return seeds;
    }

    static List<RuleSet> ParseRuleset(string[] input)
    {
        List<RuleSet> rules = new List<RuleSet>();
        Regex reg = new Regex("([0-9]+) ([0-9]+) ([0-9]+)");
        foreach (string line in input)
        {
            var matcher = reg.Match(line);
            if (matcher.Success)
            {
                var dest = long.Parse(matcher.Groups[1].Value);
                var src = long.Parse(matcher.Groups[2].Value);
                var length = long.Parse(matcher.Groups[3].Value);
                rules.Add(new RuleSet(dest, src, length));
            }
        }

        return rules;
    }

    static long GetLocation(long seed, Almanac almanac)
    {
        var val = GetValue(seed, almanac.SeedToSoil);
        val = GetValue(val, almanac.SoilToFertilizer);
        val = GetValue(val, almanac.FertilizerToWater);
        val = GetValue(val, almanac.WaterToLight); 
        val = GetValue(val, almanac.LightToTemp);
        val = GetValue(val, almanac.TempToHumidity);
        val = GetValue(val, almanac.HumidityToLocation);
        return val;
    }

    static List<Range> GetLocationRange(Range seed, Almanac alm){
        var potentialSeeds = FindPotentialResults(new List<Range>([seed]), alm.SeedToSoil);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.SoilToFertilizer);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.FertilizerToWater);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.WaterToLight);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.LightToTemp);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.TempToHumidity);
        potentialSeeds = FindPotentialResults(potentialSeeds, alm.HumidityToLocation);

        return potentialSeeds;
    }

    static long GetValue(long seed, List<RuleSet> rules)
    {
        foreach (var rule in rules)
        {
            if (seed >= rule.SourceStart && seed <= rule.SourceEnd)
            {
                var dif = seed - rule.SourceStart;
                return rule.DestinationStart + dif;
            }
        }
        return seed;
    }

    static List<Range> FindPotentialResults(List<Range> seed, List<RuleSet> rules){
        List<Range> results = new List<Range>();
        Queue<Range> seeds = new Queue<Range>();
        seed.ForEach(s => seeds.Enqueue(s));
        foreach (var rule in rules) {
            if (seeds.Count == 0) break;
            var seedCount = seeds.Count();

            for (int i = 0; i < seedCount; i++){
                var mySeed = seeds.Dequeue();
                if (rule.SourceRange.Overlap(mySeed)) {
                    var toAdd = GetCommon(rule.SourceRange, mySeed);
                    foreach (var q in Outside(mySeed, toAdd)) {
                        seeds.Enqueue(q);
                    }
                    toAdd = new Range(toAdd.start + rule.offset(), toAdd.end + rule.offset());
                    results.Add(toAdd);
                }
                else {
                    seeds.Enqueue(mySeed);
                }
            }
        }
        results.AddRange(seeds);
        return results;
    }

    static Range GetCommon(Range r1, Range r2){
        var start = Math.Max(r1.start, r2.start);
        var end = Math.Min(r1.end, r2.end);
        return new Range(start, end);
    }

    static List<Range> Outside(Range source, Range other){
        if (source.start > other.start && source.end < other.end) return new List<Range>();
        if  (source.start < other.start && source.end > other.end){
            List<Range> ret = [new Range(source.start, other.start), new Range(other.end, source.end)];
            return ret;
       }
       if(source.start < other.start){
            return [new Range(source.start, other.start)];
       }
       if(source.end > other.end){
            return [new Range(other.end, source.end)];
       }
        return new List<Range>();
    }
}

