using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GearRatios;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        //SolvePart1(input);
        SolvePart2(input);
    }

    static void SolvePart1(string[] input){
        var result = new List<int>();
        foreach(var p in FindSymbols(input)){
            result.AddRange(FindAdjacentNumbers(input, p));
        }
        Console.WriteLine("Part 1: " + result.Sum());
    }

    static void SolvePart2(string[] input){
        var result = new List<int>();
        checkd = new List<Point>();
        foreach(var p in FindGears(input)){
           var numbers = FindAdjacentNumbers(input, p);
           if(numbers.Count == 2){
                result.Add(numbers[0] * numbers[1]);
           }
        }
        Console.WriteLine("Part 2: " + result.Sum());
    }

    public static IEnumerable<Point> FindSymbols(string[] input){
        for (int y = 0; y < input.Length; y++){
            for(int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '.' || Char.IsDigit(input[y][x])) continue;
                yield return new Point(x, y);
            }
        }
    }

    public static IEnumerable<Point> FindGears(string[] input){
        for (int y = 0; y < input.Length; y++){
            for(int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '*') 
                    yield return new Point(x, y);
            }
        }
    }

    static Point[] adjacents = new Point[]{
        new Point(0, 1),
        new Point(1, 0),
        new Point(0, -1),
        new Point(-1, 0),
        new Point(1, 1),
        new Point (-1, 1),
        new Point (1, -1),
        new Point(-1, -1)
    };

    static List<Point> checkd = new List<Point>();
    public static List<int> FindAdjacentNumbers(string[] input, Point symbol){
        var results = new List<int>();
        foreach(Point a in adjacents){
            var newPoint = symbol + a;
            if (checkd.Contains(newPoint)) continue;
            
            if(Char.IsDigit(input[newPoint.y][newPoint.x])){
                checkd.Add(newPoint);
                var sb = new StringBuilder();
                sb.Append(input[newPoint.y][newPoint.x]);
                var newX = newPoint.x + 1;
                while(newX < input[newPoint.y].Length && char.IsDigit(input[newPoint.y][newX])){
                    if (checkd.Contains(new Point(newX, newPoint.y))) break;
                    checkd.Add(new Point(newX, newPoint.y));
                    sb.Append(input[newPoint.y][newX]);
                    newX = newX + 1;
                }
                newX = newPoint.x - 1;
                while(newX >= 0 && char.IsDigit(input[newPoint.y][newX])){
                    if (checkd.Contains(new Point(newX, newPoint.y))) break;
                    checkd.Add(new Point(newX, newPoint.y));
                    sb.Insert(0, input[newPoint.y][newX]);
                    newX = newX - 1;
                }
                results.Add(int.Parse(sb.ToString()));
            }
        }
        return results;
    }
}

public record Point(int x, int y){
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
};
