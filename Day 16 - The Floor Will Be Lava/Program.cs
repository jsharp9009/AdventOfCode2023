using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheFloorWillBeLava;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").Select(s => s.ToCharArray()).ToArray();
        SolvePart1(input);
        SolvePart2(input);
    }

    static void SolvePart1(char[][] input){
        Console.WriteLine("Part 1: " + FindEnergized(input, new Point(0, 0), Directions.East));
    }

    static void SolvePart2(char[][] input){
        var maxEnergized = 0;
        for(int i = 0; i < input.Length; i++){
            var newEnergy = FindEnergized(input, new Point(i, 0), Directions.East);
            if (maxEnergized < newEnergy) maxEnergized = newEnergy;
            newEnergy = FindEnergized(input, new Point(i, input[i].Length - 1), Directions.West);
            if (maxEnergized < newEnergy) maxEnergized = newEnergy;
        }

        for(int i = 0; i < input[0].Length; i++){
            var newEnergy = FindEnergized(input, new Point(0, i), Directions.South);
            if (maxEnergized < newEnergy) maxEnergized = newEnergy;
            newEnergy = FindEnergized(input, new Point(input.Length - 1, i), Directions.North);
            if (maxEnergized < newEnergy) maxEnergized = newEnergy;
        }

        Console.WriteLine("Part 2: " + maxEnergized);
    }

    static int FindEnergized(char[][] input, Point start, Point startDirection){
        Queue<Bot> bots = new Queue<Bot>();
        var startDirections = GetDirectionFromChar(input[start.row][start.column], startDirection);
        foreach(var dir in startDirections){
            bots.Enqueue(new Bot(new Point(start.row, start.column), dir));
        }
        
        var energized = new List<Bot>();
        while(bots.Count > 0){
            var bot = bots.Dequeue();
            if (energized.Contains(bot)) continue;
            energized.Add(bot);
            var newLocation = bot.currentPoint + bot.direction;
            if (newLocation.row < 0 || newLocation.row >= input.Length || newLocation.column < 0 || newLocation.column >= input[0].Length) continue;
            var newChar = input[newLocation.row][newLocation.column];
            var newDirections = GetDirectionFromChar(newChar, bot.direction);
            foreach(var dir in newDirections){
                bots.Enqueue(new Bot(newLocation, dir));
            }
        }
        //PrintGrid(input, energized);
        return energized.Select(c => c.currentPoint).Distinct().Count();
    }

    static Point[] GetDirectionFromChar(char newChar, Point currentDirection){
        if(newChar == '.'){
                return [currentDirection];
            }
            if(newChar == '\\'){
                return [Directions.BackslashChange[currentDirection]];
            }
            if(newChar == '/'){
                return [Directions.ForwardslashChange[currentDirection]];
            }
            if(newChar == '-'){
                return Directions.DashChange[currentDirection];
            }
            if(newChar == '|'){
                return Directions.PipeChange[currentDirection];
            }
        return Array.Empty<Point>();
    }

    static void PrintGrid(char[][] input, List<Bot> energized){
        for(int row = 0; row < input.Length; row++){
            for(int column = 0; column < input[row].Length; column++){
                var newPoint = new Point(row, column);
                if (energized.Any(p => p.currentPoint == newPoint ))
                    Console.Write("#");
                else
                    Console.Write(input[row][column]);
            }
            Console.WriteLine();
        }
    }
}

record Point(int row, int column){
    public static Point operator +(Point a, Point b){
        return new Point(a.row + b.row, a.column + b.column);
    }
};

record Bot(Point currentPoint, Point direction);

static class Directions{
    public static readonly Point South = new Point(1, 0);
    public static readonly Point North = new Point(-1, 0);
    public static readonly Point East = new Point(0, 1);
    public static readonly Point West = new Point(0, -1);

    public static readonly Dictionary<Point, Point> BackslashChange = new Dictionary<Point, Point>
    {
        {Directions.North, Directions.West},
        {Directions.South, Directions.East},
        {Directions.West, Directions.North},
        {Directions.East, Directions.South}
    };

    public static readonly Dictionary<Point, Point> ForwardslashChange = new Dictionary<Point, Point>
    {
        {Directions.North, Directions.East},
        {Directions.South, Directions.West},
        {Directions.East, Directions.North},
        {Directions.West, Directions.South}
    };

    public static Dictionary<Point, Point[]> DashChange = new Dictionary<Point, Point[]>
    {
        {Directions.North, [Directions.East, Directions.West]},
        {Directions.South, [Directions.East, Directions.West]},
        {Directions.East, [Directions.East]},
        {Directions.West, [Directions.West]}
    };

    public static Dictionary<Point, Point[]> PipeChange = new Dictionary<Point, Point[]>
    {
        {Directions.North, [Directions.North]},
        {Directions.South, [Directions.South]},
        {Directions.East, [Directions.South, Directions.North]},
        {Directions.West, [Directions.South, Directions.North]}
    };
}