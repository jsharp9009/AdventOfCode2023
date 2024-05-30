using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClumsyCrucible;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").Select(s => s.ToCharArray()).Select(c => Array.ConvertAll(c, b => int.Parse(b.ToString()))).ToArray();

        var answer = SolvePart1(input);
        Console.WriteLine("Part 1: " + answer);

        answer = SolvePart2(input);
        Console.WriteLine("Part 2: " + answer);
    }

    static int SolvePart1(int[][] chitonMap)
    {
        var height = chitonMap.Length;
        var width = chitonMap[0].Length;

        Dictionary<State, int> distances = new Dictionary<State, int>();
        PriorityQueue<State, int> queue = new PriorityQueue<State, int>();

        queue.Enqueue(new State(new Point(0, 0), new Point(0, 1), 0), 0);
        queue.Enqueue(new State(new Point(0, 0), new Point(1, 0), 0), 0);
        distances.Add(new State(new Point(0, 0), new Point(1, 0), 0), 0);
        distances.Add(new State(new Point(0, 0), new Point(0, 1), 0), 0);


        while (queue.Count > 0)
        {
            var lowest = queue.Dequeue();
            var lowestDistance = distances[lowest];
            foreach (Point newVelocity in Rotations(lowest.Velocity))
            {
                var next = lowest.Current + newVelocity;

                if (next.x >= width || next.x < 0 || next.y >= height || next.y < 0) continue;
                var newDistance = lowestDistance + chitonMap[next.y][next.x];

                var nextState = new State(next, newVelocity, newVelocity == lowest.Velocity ? lowest.distance + 1 : 1);

                if(next.x == width - 1 && next.y == height - 1 && nextState.distance < 4){
                    //Console.WriteLine(nextState + " " + newDistance);
                    return newDistance;
                }

                if(nextState.distance < 4 && !distances.ContainsKey(nextState)){
                    distances.Add(nextState, newDistance);
                    queue.Enqueue(nextState, newDistance);
                    //Console.WriteLine(nextState + " " + newDistance);
                }
            }
        }
        return int.MaxValue;
    }

    static int SolvePart2(int[][] chitonMap)
    {
        var height = chitonMap.Length;
        var width = chitonMap[0].Length;

        Dictionary<State, int> distances = new Dictionary<State, int>();
        PriorityQueue<State, int> queue = new PriorityQueue<State, int>();

        queue.Enqueue(new State(new Point(0, 0), new Point(0, 1), 0), 0);
        queue.Enqueue(new State(new Point(0, 0), new Point(1, 0), 0), 0);
        distances.Add(new State(new Point(0, 0), new Point(1, 0), 0), 0);
        distances.Add(new State(new Point(0, 0), new Point(0, 1), 0), 0);


        while (queue.Count > 0)
        {
            var lowest = queue.Dequeue();
            var lowestDistance = distances[lowest];
            foreach (Point newVelocity in Rotations(lowest.Velocity))
            {
                if (lowest.Velocity == newVelocity && lowest.distance >= 10) continue;
                if (lowest.Velocity != newVelocity && lowest.distance < 4) continue;

                var next = lowest.Current + newVelocity;

                if (next.x >= width || next.x < 0 || next.y >= height || next.y < 0) continue;
                var newDistance = lowestDistance + chitonMap[next.y][next.x];

                var nextState = new State(next, newVelocity, newVelocity == lowest.Velocity ? lowest.distance + 1 : 1);

                if(next.x == width - 1 && next.y == height - 1 && nextState.distance >= 4){
                    //Console.WriteLine(nextState + " " + newDistance);
                    return newDistance;
                }

                if(!distances.ContainsKey(nextState)){
                    distances.Add(nextState, newDistance);
                    queue.Enqueue(nextState, newDistance);
                    //Console.WriteLine(nextState + " " + newDistance);
                }
            }
        }
        return int.MaxValue;
    }

    public static IEnumerable<Point> Rotations(Point velocity){
        yield return new Point(velocity.y, velocity.x * -1);
        yield return new Point(velocity.y * -1, velocity.x);
        yield return velocity;
    }
}

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
};


public record State(Point Current, Point Velocity, int distance){
    public override string ToString()
    {
        return Current.ToString() + " " + Velocity.ToString() + " " + distance.ToString();
    }
}