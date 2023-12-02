using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CubeCunundrum;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        SolvePart1(input);
        SolvePart2(input);
    }

    private static void SolvePart1(string[] input){
        var answer = 0;
        for(int i = 0; i < input.Length; i++){
            if (CheckIfPossible(input[i], 12, 13, 14))
                answer += i + 1;
        }
        Console.WriteLine("Part 1: " + answer);
    }

    private static void SolvePart2(string[] input){
        var answer = 0;
        foreach(var line in input){
            answer += GetPower(line);
        }
        Console.WriteLine("Part 2: " + answer);
    }

    private static bool CheckIfPossible(string pulls, int red, int green, int blue){
        var redCount = GetMax("red", pulls);
        if (redCount > red) return false;

        var greenCount = GetMax("green", pulls);
        if (greenCount > green) return false;

        var blueCount = GetMax("blue", pulls);
        if (blueCount > blue) return false;

        return true;
    }

    private static int GetPower(string pulls){
        var redCount = GetMax("red", pulls);

        var greenCount = GetMax("green", pulls);

        var blueCount = GetMax("blue", pulls);

        return redCount * greenCount * blueCount;
    }

    private static int GetMax(string color, string pulls){
        var regex = new Regex($"([0-9]*) {color}");
        var match = regex.Match(pulls);
        var max = 0;
        while(match.Success){
            var group = match.Groups[1];
            var count = int.Parse(group.Value);
            if (count > max) max = count;
            match = match.NextMatch();
        }
        return max;
    }

    private static int GetMin(string color, string pulls){
        var regex = new Regex($"([0-9]*) {color}");
        var match = regex.Match(pulls);
        var min = int.MaxValue;
        while(match.Success){
            var group = match.Groups[1];
            var count = int.Parse(group.Value);
            if (count < min) min = count;
            match = match.NextMatch();
        }
        return min;
    }

    
}
