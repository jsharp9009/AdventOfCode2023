using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Trebuchette;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        SolvePart1(input);
        SolvePart2(input);
    }

    static void SolvePart1(string[] input){
        var allNumbers = new int[input.Length];
        for (int i = 0; i < input.Length; i++) {
            var line = input[i];
            var first = GetFirstDigit(line);
            var last = GetLastDigit(line);
            allNumbers[i] = (first * 10) + last;
        }
        Console.WriteLine($"Sum of all Calibration numbers Part 1: {allNumbers.Sum()}");
    }

    static void SolvePart2(string[] input){
        var allNumbers = new int[input.Length];
        for (int i = 0; i < input.Length; i++) {
            var line = input[i];
            var first = GetFirstDigitOrWord(line);
            var last = GetLastDigitOrWord(line);
            var result = (first * 10) + last;
            //Console.WriteLine(line + ": " + result);
            allNumbers[i] = result;
        }
        Console.WriteLine($"Sum of all Calibration numbers Part 2: {allNumbers.Sum()}");
    }

    static int GetFirstDigit(string line) => int.Parse(line.First(char.IsNumber).ToString());

    static int GetLastDigit(string line) => int.Parse(line.Reverse().First(char.IsNumber).ToString());

    static int GetFirstDigitOrWord(string line){
        var sb = new StringBuilder();
        foreach(char c in line){
            if (Char.IsDigit(c)) return int.Parse(c.ToString());

            sb.Append(c);
            if (sb.ToString().Contains("one")) return 1;
            if (sb.ToString().Contains("two")) return 2;
            if (sb.ToString().Contains("three")) return 3;
            if (sb.ToString().Contains("four")) return 4;
            if (sb.ToString().Contains("five")) return 5;
            if (sb.ToString().Contains("six")) return 6;
            if (sb.ToString().Contains("seven")) return 7;
            if (sb.ToString().Contains("eight")) return 8;
            if (sb.ToString().Contains("nine")) return 9;           
        }
        return 0;
    }

    static int GetLastDigitOrWord(string line){
        var sb = new StringBuilder();
        for(int i = line.Length - 1; i >= 0; i--){
            var c = line[i];
            if (Char.IsDigit(c)) return int.Parse(c.ToString());

            sb.Insert(0, c);
            if (sb.ToString().Contains("one")) return 1;
            if (sb.ToString().Contains("two")) return 2;
            if (sb.ToString().Contains("three")) return 3;
            if (sb.ToString().Contains("four")) return 4;
            if (sb.ToString().Contains("five")) return 5;
            if (sb.ToString().Contains("six")) return 6;
            if (sb.ToString().Contains("seven")) return 7;
            if (sb.ToString().Contains("eight")) return 8;
            if (sb.ToString().Contains("nine")) return 9;           
        }
        return 0;
    }
    
}
