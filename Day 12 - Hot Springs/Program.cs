using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HotSprings;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var records = ParseInput(input);
        SolvePart1(records);
        SolvePart2(records);
    }

    static void SolvePart1(List<Record> records){
        Console.WriteLine("Part 1: " + records.Sum(c => Count(c, new Dictionary<int, long>(), 0)));
    }

    static void SolvePart2(List<Record> records){
        Console.WriteLine("Part 2: " + records.Sum(c => Count(c, 5)));
    }

    static List<Record> ParseInput(string[] input){
        var records = new List<Record>();
        foreach(var line in input){
            var split1 = line.Split(" ");
            records.Add(new Record(split1[0], split1[1].Split(",").Select(c => int.Parse(c)).ToList()));
        }
        return records;
    }

    static long Count(Record record, int duplication){
        string newCondition = string.Join("?", Enumerable.Repeat(record.Condition, duplication));
        var newGroups = Enumerable.Repeat(record.groups, duplication).SelectMany(v => v).ToList();
        return Count(new Record(newCondition, newGroups), new Dictionary<int, long>(), 0);
    }

    static long Count(Record record, Dictionary<int, long> counts, int key){
        long count = 0;
        if (counts.ContainsKey(key)) return counts[key];
        if (record.groups.Count == 0)
            return counts[key] = record.Condition.Any(c => c == '#') ? 0 : 1;
        int currentGroup = record.groups[0];
        int max = record.Condition.Length - record.groups.Count - Math.Max(currentGroup, record.groups.Sum() - 1);
        int nonEmpty = record.Condition[..record.groups[0]].Count(c => c != '.');
        for(int first = 0, last = currentGroup; first <= max;){
            char firstChar = record.Condition[first++];
            char lastChar = record.Condition[last++];
            if (nonEmpty == currentGroup && lastChar != '#')
                count += Count(new Record(record.Condition[last..], record.groups[1..]), counts, key + last * 32 + 1);
            if (firstChar == '#') return counts[key] = count;
            nonEmpty += (lastChar == '.' ? 0 : 1) -(firstChar == '.' ? 0 : 1);
        }
        if (nonEmpty == currentGroup && record.groups.Count == 1) count++;
        return counts[key] = count;
    }
}

record Record(string Condition, List<int> groups);