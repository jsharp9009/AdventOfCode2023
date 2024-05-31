using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LavaductLagoon;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var digPlan = ReadDigPlanPart1(input);
        (var corners, var parimeter) = GetCorners(digPlan);
        var answer = GetArea(corners, parimeter);
        Console.WriteLine("Part 1: " + answer);

        digPlan = ReadDigPlanPart2(input);
        (corners, parimeter) = GetCorners(digPlan);
        answer = GetArea(corners, parimeter);
        Console.WriteLine("Part 2: " + answer);
    }

    static List<DigInstruction> ReadDigPlanPart1(string[] input)
    {
        var output = new List<DigInstruction>();
        foreach (var line in input)
        {
            var parts = line.Split(" ");
            var direction = parts[0][0];
            var count = int.Parse(parts[1]);
            var hex = parts[2].Replace("(", "").Replace(")", "");
            output.Add(new DigInstruction(direction, count, hex));
        }
        return output;
    }

    static List<DigInstruction> ReadDigPlanPart2(string[] input)
    {
        var output = new List<DigInstruction>();
        foreach (var line in input)
        {
            var parts = line.Split(" ");
            var hex = parts[2].Replace("(#", "").Replace(")", "");
            var count = Convert.ToInt64(hex.Substring(0, 5), 16);
            var direction = intDirections[int.Parse(hex.Last().ToString())];
            output.Add(new DigInstruction(direction, count, hex));
        }
        return output;
    }

    static (List<Point>, long) GetCorners(List<DigInstruction> instructions)
    {
        Point current = new Point(0, 0);
        var trench = new List<Point> {  };
        var parimeter = 0L;
        foreach (var instruction in instructions)
        {
            var direction = directions[instruction.Direction];
            current = current + (direction * instruction.count);
            parimeter += instruction.count;
            if (!trench.Contains(current))
                trench.Add(current);
        }
        return new(trench, parimeter);
    }

    static Dictionary<char, Point> directions = new Dictionary<char, Point>(){
        {'U', new Point(0, -1)},
        {'D', new Point(0, 1)},
        {'L', new Point(-1, 0)},
        {'R', new Point(1, 0)},
    };

    static Dictionary<int, char> intDirections = new Dictionary<int, char>(){
        { 0, 'R'},
        { 1, 'D'},
        { 2, 'L'},
        { 3, 'U'}
    };

    static long GetArea(List<Point> corners, long parimeter)
    {
        var vertices = corners.Count - 1;
        var area = 0L;
        for (int i = 0; i < vertices; i++)
        {
            area += (corners[i].x * corners[i + 1].y) - (corners[i + 1].x * corners[i].y);
        }
        area += (corners[vertices].x * corners[0].y) - (corners[0].x * corners[vertices].y);
        area = Math.Abs(area) / 2;
        area += (parimeter / 2) + 1;
        return area;
    }
}

public record DigInstruction(char Direction, long count, string hexColor);

public record Point(long x, long y)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point operator *(Point a, long b)
    {
        return new Point(a.x * b, a.y * b);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
};
