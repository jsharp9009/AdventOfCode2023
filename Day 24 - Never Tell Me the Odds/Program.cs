using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NeverTellMeTheOdds;

/// <summary>
/// Mostly borrowed code from https://aoc.csokavar.hu/?day=24 because I'm bad at Geometry
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var rays = ParseInput(input);
        var intersections = CountIntersections(rays, 200000000000000, 400000000000000);
        Console.WriteLine("Part 1: " + intersections);

        var xy = Solve2D(Project(rays, v => new Point(v.x, v.y, 0)).ToList());
        var xz = Solve2D(Project(rays, v => new Point(v.x, v.z, 0)).ToList());
        var answer = Math.Round(xy.x + xy.y + xz.y);
        Console.WriteLine("Part 2: " + answer);
    }

    static int CountIntersections(List<Ray> rays, long min, long max)
    {
        int count = 0;
        for (int i = 0; i < rays.Count - 1; i++)
        {
            for (int n = i + 1; n < rays.Count; n++)
            {
                var intersect = FindIntersection(rays[i], rays[n]);
                if (intersect == null) continue;
                if (!InFuture(rays[i], intersect) || !InFuture(rays[n], intersect)) continue;
                if (intersect.x < min || intersect.x > max) continue;
                if (intersect.y < min || intersect.y > max) continue;
                count++;
            }
        }
        return count;
    }

    static List<Ray> ParseInput(string[] input)
    {
        List<Ray> result = new List<Ray>();
        foreach (var line in input)
        {
            var matches = Regex.Match(line, "[ ]*(-*\\d+),[ ]*(-*\\d+),[ ]*(-*\\d+)[ ]*@[ ]*(-*\\d+),[ ]*(-*\\d+),[ ]*(-*\\d+)");
            if (matches.Success)
            {
                result.Add(new Ray(new Point(decimal.Parse(matches.Groups[1].Value), decimal.Parse(matches.Groups[2].Value), decimal.Parse(matches.Groups[3].Value)),
                new Point(decimal.Parse(matches.Groups[4].Value), decimal.Parse(matches.Groups[5].Value), decimal.Parse(matches.Groups[6].Value))));
            }
        }
        return result;
    }

    public static Point? FindIntersection(Ray r1, Ray r2)
    {
        var determinate = r1.Velocity.x * r2.Velocity.y - r1.Velocity.y * r2.Velocity.x;
        if (determinate == 0) return null;

        var c1 = r1.Velocity.x * r1.Start.y - r1.Velocity.y * r1.Start.x;
        var c2 = r2.Velocity.x * r2.Start.y - r2.Velocity.y * r2.Start.x;

        var x = (r2.Velocity.x * c1 - r1.Velocity.x * c2) / determinate;
        var y = (r2.Velocity.y * c1 - r1.Velocity.y * c2) / determinate;

        return new Point(x, y, 0);
    }

    public static bool InFuture(Ray start, Point next)
    {
        return Math.Sign(next.x - start.Start.x) == Math.Sign(start.Velocity.x);
    }

    public static Point? Solve2D(List<Ray> rays)
    {
        var limit = 500;
        for (var vx = -limit; vx < limit; vx++)
        {
            for (var vy = -limit; vy < limit; vy++)
            {
                var Velocity = new Point(vx, vy, 0);
                var stone = FindIntersection(TranslateV(rays[0], Velocity), TranslateV(rays[1], Velocity));
                if (stone == null) continue;
                if (rays.All(p => Hits(TranslateV(p, Velocity), stone)))
                {
                    return stone;
                }
            }
        }
        return null;
    }

    public static Ray TranslateV(Ray p, Point v)
    {
        return new Ray(p.Start, new Point(p.Velocity.x - v.x, p.Velocity.y - v.y, p.Velocity.z - v.z));
    }

    public static bool Hits(Ray r1, Point pos)
    {
        var d = (pos.x - r1.Start.x) * r1.Velocity.y - (pos.y - r1.Start.y) * r1.Velocity.x;
        return Math.Abs(d) < .0001m;
    }

    static IEnumerable<Ray> Project(List<Ray> ps, Func<Point, Point> proj)
    {
        // from p in ps select new Ray(
        //     new Point(proj(p.Start).Item1, proj(p.Start).Item2, 0),
        //     new Point(proj(p.Velocity).Item1, proj(p.Velocity).Item2, 0)
        // )

        foreach (var p in ps)
        {
            yield return new Ray(
                new Point(proj(p.Start).x, proj(p.Start).y, 0),
                new Point(proj(p.Velocity).x, proj(p.Velocity).y, 0)
            );
        }

    }
}

record Point(decimal x, decimal y, decimal z)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y, a.z - b.z);
    }
}
record Ray(Point Start, Point Velocity)
{
}