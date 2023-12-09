using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MirageMaintenance;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var history = ParseInput(input);
        SolvePart1(history);
        SolvePart2(history);
    }

    static void SolvePart1(List<List<int>> history){
        Console.WriteLine("Part 1: " + history.Select(h => h.Last() + GetNextValue(h)).Sum());
    }

    static void SolvePart2(List<List<int>> history){
        history.ForEach(h => h.Reverse());
        Console.WriteLine("Part 2: " + history.Select(h => h.Last() + GetNextValue(h)).Sum());
    }

    public static int GetNextValue(IEnumerable<int> input){
        var nextStep = input.Zip(input.Skip(1), (i, n) => n - i);
        if (nextStep.Distinct().Count() == 1)
            return nextStep.First();
        return nextStep.Last() + GetNextValue(nextStep);
    }

    static List<List<int>> ParseInput(string[] input){
        var reg = new Regex("(-*[0-9]+)");
        var ret = new List<List<int>>();
        
        foreach(var line in input){
            List<int> row = new List<int>();
            var matcher = reg.Matches(line);
            for(int i = 0; i < matcher.Count; i++){
                row.Add(int.Parse(matcher[i].Value));
            }
            ret.Add(row);
        }
        return ret;
    }
}
