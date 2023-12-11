using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace PipeMaze;

class Program
{
    static Dictionary<char, Point[]> CharMap = new Dictionary<char, Point[]>{
        {'|', [Directions.North, Directions.South]},
        {'-', [Directions.East, Directions.West]},
        {'L', [Directions.North, Directions.East]},
        {'J', [Directions.North, Directions.West]},
        {'7', [Directions.West, Directions.South]},
        {'F', [Directions.South, Directions.East]},
        {'.', []}
    };

    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var maze = input.Select(s => s.ToCharArray()).ToArray();
        var start = FindStartingPoint(maze);
        var startingChar = FindStartChar(maze, start);
        maze[start.row][start.column] = startingChar;
        var pipe = FindPipe(maze, start);
        //PrintMaze(maze, pipe.Select(p => p.Position).ToList());
        Console.WriteLine("Part 1: " + pipe.Max(p => p.steps));
        Console.WriteLine("Part 2: " + FindTrappedSpaces(maze, pipe.Select(p => p.Position).ToList()));

    }

     static int FindTrappedSpaces(char[][] maze, List<Point> pipe){
        List<Point> trapped = new List<Point>();
        for (int column = 0; column < maze[0].Length; column++){
            int canGoEast = 0;
            for(int row = 0; row < maze.Length; row++){
                if(pipe.Contains(new Point(column, row))){
                    var c = maze[row][column];
                    if(CharMap[c].Contains(Directions.East)){
                        canGoEast++;
                    }
                }
                else{
                    if(canGoEast % 2 == 1){
                        trapped.Add(new Point(column, row));
                    }
                }
            }
        }
        //PrintMaze(maze, pipe, trapped);
        return trapped.Count();
     }

    static char FindStartChar(char[][] maze, Point starting){
        var canEnterFrom = new List<Point>();
        var directions = new Point[] { Directions.North, Directions.South, Directions.East, Directions.West };
        foreach (Point dir in directions){
            var possible = starting + dir;
            if (possible.row < 0 || possible.column < 0) continue;
            if( CharMap[maze[possible.row][possible.column]].Contains(dir * -1)){
                canEnterFrom.Add(dir);
            }
        }

        return CharMap.Where(v => v.Value.OrderBy(x => x.row).OrderBy(x => x.column).SequenceEqual(canEnterFrom.OrderBy(y => y.row).OrderBy(y => y.column))).Select(c => c.Key).First();
    }

    static Point FindStartingPoint(char[][] maze)
    {
        for (int row = 0; row < maze.Length; row++)
        {
            for (int column = 0; column < maze.Length; column++)
            {
                if (maze[row][column] == 'S') return new Point(column, row);
            }
        }
        return new Point(0, 0);
    }

    static List<State> FindPipe(char[][] maze, Point starting)
    {
        var startingState = new State(starting, new Point(0, 0), 0);
        Queue<State> checking = new Queue<State>();
        checking.Enqueue(startingState);
        List<State> checkd = new List<State>() { startingState };

        while(checking.Any()){
            var current = checking.Dequeue();
            var possibleDirections = CharMap[maze[current.Position.row][current.Position.column]];
            foreach(Point p in possibleDirections){
                if (p == current.EnteredFrom) continue;
                if (checkd.Any(c => c.Position == (current.Position + p))) continue;
                var toCkeck = new State(current.Position + p, p * -1, current.steps + 1);
                checking.Enqueue(toCkeck);
                checkd.Add(toCkeck);
            }
        }

        return checkd;
    }

    static void PrintMaze(char[][] maze, List<Point> pipes, List<Point>? trapped = null){
        for(int row = 0; row < maze.Length; row++ ){
            for(int column = 0; column < maze[row].Length; column++){
                if (pipes.Any(p => p.row == row && p.column == column))
                    Console.ForegroundColor = ConsoleColor.Red;
                if (trapped != null && trapped.Any(p => p.row == row && p.column == column))
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.Write(maze[row][column]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}

record Point(int column, int row)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.column + b.column, a.row + b.row);
    }

    public static Point operator *(Point a, int change)
    {
        return new Point(a.column * change, a.row * change);
    }
}

static class Directions
{
    public static Point North = new Point(0, -1);
    public static Point South = new Point(0, 1);
    public static Point East = new Point(1, 0);
    public static Point West = new Point(-1, 0);
}

record State(Point Position, Point EnteredFrom, int steps);