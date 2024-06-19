using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ALongWalk;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").Select(s => s.ToArray()).ToArray();
        var height = input.Length;
        var width = input[0].Length;

        var intersections = FindIntersections(input, height, width);
        var connections = FindConnections(intersections, input);
        var answer = GetLongestPath(connections, height, width);
        Console.WriteLine("Part 1: " + answer);

        connections = FindConnections(intersections, input, false);
        answer = GetLongestPath(connections, height, width);
        Console.WriteLine("Part 2: " + answer);
    }

    static List<Point> FindIntersections(char[][] input, int height, int width)
    {
        List<Point> corners = new List<Point>();

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (input[y][x] == '#') continue;
                var current = new Point(x, y);
                var dirSum = 0;
                if (input[y - 1][x] != '#' && input[y][x - 1] != '#') dirSum++;
                if (input[y - 1][x] != '#' && input[y][x + 1] != '#') dirSum++;
                if (input[y + 1][x] != '#' && input[y][x - 1] != '#') dirSum++;
                if (input[y + 1][x] != '#' && input[y][x + 1] != '#') dirSum++;

                if (dirSum > 1) corners.Add(current);
            }
        }

        corners.Add(new Point(1, 0));
        corners.Add(new Point(width - 2, height - 1));

        return corners;
    }

    static Dictionary<Point, List<Connection>> FindConnections(List<Point> corners, char[][] input, bool slip = true)
    {
        corners = corners.OrderBy(c => c.x).ThenBy(c => c.y).ToList();
        Dictionary<Point, List<Connection>> connections = new Dictionary<Point, List<Connection>>();
        var height = input.Length - 1;
        var width = input[0].Length - 1;
        foreach (var cor in corners)
        {
            connections.Add(cor, new List<Connection>());
            foreach (var dir in directions)
            {
                var current = cor;
                var direction = dir;
                var count = 0;
                while (true)
                {
                    var test = current + direction;
                    if (test.x < 0 || test.y < 0 || test.x > width || test.y > height) break;
                    if (input[test.y][test.x] == '#')
                    {
                        if (current == cor) break;
                        foreach (var d in directions)
                        {
                            if (d == direction) continue;
                            if (d.x == direction.x * -1 && d.y == direction.y * -1) continue;

                            test = current + d;
                            if (test.x < 0 || test.y < 0 || test.x > width || test.y > height) continue;
                            if (input[test.y][test.x] == '#') continue;

                            direction = d;
                            break;
                        }
                    }

                    if (slip)
                    {
                        if (input[current.y][current.x] == 'v' && direction != directions[2])
                            break;
                        if (input[current.y][current.x] == '^' && direction != directions[3])
                            break;
                        if (input[current.y][current.x] == '<' && direction != directions[1])
                            break;
                        if (input[current.y][current.x] == '>' && direction != directions[0])
                            break;
                    }

                    if (corners.Contains(test))
                    {
                        connections[cor].Add(new Connection(test, count + 1));
                        break;
                    }

                    count++;
                    current = test;
                }
            }
        }

        return connections;
    }


    static int GetLongestPath(Dictionary<Point, List<Connection>> connections, int height, int width)
    {
        var target = new Point(width - 2, height - 1);
        var start = new Point(1, 0);
        var que = new Queue<Path>();
        que.Enqueue(new Path(start, 0, new HashSet<Point>() { start }));
        var maxLength = 0;
        while (que.TryDequeue(out var path))
        {
            foreach (var conn in connections[path.current])
            {
                if (conn.destination == target)
                {
                    var length = path.length + conn.length;
                    if (length > maxLength)
                        maxLength = length;
                    break;
                }
                if (path.visited.Contains(conn.destination)) continue;

                var newPath = new HashSet<Point>(path.visited) { conn.destination };
                var np = new Path(conn.destination, path.length + conn.length, newPath);
                que.Enqueue(np);
            }
        }

        return maxLength;
    }
    static Point[] directions = new Point[]{
        new Point(1, 0),
        new Point(-1, 0),
        new Point(0, 1),
        new Point(0, -1)
    };
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

public record Connection(Point destination, int length);
public record Path(Point current, int length, HashSet<Point> visited);
