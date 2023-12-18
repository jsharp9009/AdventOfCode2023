using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LensLibrary;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").SelectMany(s => s.Split(",")).ToArray();
        SolvePart1(input);
        SolvePart2(input);
    }

    static void SolvePart1(string[] input){
        Console.WriteLine("Part 1: " + input.Sum(s => GetHash(s)));
    }

    static void SolvePart2(string[] input){
        Dictionary<int, List<Lens>> boxes = new Dictionary<int, List<Lens>>();
        var regex = new Regex(@"([A-z]*)(\=|\-)([0-9]*)");
        foreach(string line in input){
            var matches = regex.Match(line);
            var label = matches.Groups[1].Value;
            var hash = GetHash(label);
            if(matches.Groups[2].Value == "="){
                if(!boxes.ContainsKey(hash)){
                    boxes.Add(hash, new List<Lens>());
                }
                var foundLense = boxes[hash].FirstOrDefault(a => a.Label == label);
                if (foundLense == default)
                {
                    var lens = new Lens(label, int.Parse(matches.Groups[3].Value));
                    boxes[hash].Add(lens);
                }
                else{
                    var index = boxes[hash].IndexOf(foundLense);
                    boxes[hash][index] = new Lens(label, int.Parse(matches.Groups[3].Value));
                }
            }
            else if (matches.Groups[2].Value == "-"){
                if (!boxes.ContainsKey(hash)) continue;
                var rmv = boxes[hash].FirstOrDefault(a => a.Label == label);
                if (rmv != default) boxes[hash].Remove(rmv);
            }
        }

        var sum = 0;
        foreach(var pair in boxes){
            for(int i = 0; i < pair.Value.Count; i++){
                sum += (pair.Key + 1) * (i + 1) * pair.Value[i].Power;
            }
        }

        Console.WriteLine("Part 2: " + sum);
    }

    static int GetHash(string value){
        var hash = 0;
        foreach(char c in value){
            hash += (int)c;
            hash *= 17;
            hash %= 256;
        }
        return hash;
    }

}

public record Lens(string Label, int Power){

};
