using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StepCounter;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").Select(s => s.ToCharArray()).ToArray();
        var start = new Point((int)Math.Floor(input[0].Length / 2d), (int)Math.Floor(input.Length / 2d));

        var width = input[0].Length;
        var halfWidth = width / 2;

        var counts = CountPlots(input, start, halfWidth + width * 2);
        Console.WriteLine("Part 1: " + 64);

        var halfWidthCount = counts[halfWidth];
        var widthPlusHalfCount = counts[halfWidth + width];

        var d1 = widthPlusHalfCount - halfWidthCount;
        var d2 = counts[halfWidth + width * 2] - widthPlusHalfCount;
        var q = 26501365L / width;

        var answer = halfWidthCount + d1 * q + q * (q - 1) / 2 * (d2 - d1);
        Console.WriteLine("Part 2: " + answer);
    }

    public static Dictionary<int, int> CountPlots(char[][] field, Point start, int steps)
    {
        var plots = new HashSet<Point>() { start };
        var plotsToSearch = new HashSet<Point> { };
        var plotCounts = new Dictionary<int, int>();
        var height = field.Length;
        var width = field[0].Length;
        Dictionary<Point, HashSet<Point>> memory = new Dictionary<Point, HashSet<Point>>();

        for (int i = 0; i < steps; i++)
        {
            (plots, plotsToSearch) = (plotsToSearch, plots);
            plots.Clear();
            foreach (var plot in plotsToSearch)
            {
                if (memory.ContainsKey(plot))
                {
                    foreach (var addPlot in memory[plot])
                    {
                        if (!plots.Contains(addPlot))
                        {
                            plots.Add(addPlot);
                        }
                    }
                    continue;
                }
                foreach (var directiion in directions)
                {
                    var newSpot = plot + directiion;
                    var testX = newSpot.x;
                    var testY = newSpot.y;
                    if (testX < 0)
                    {
                        testX = ((testX % width) + width) % width;
                    }
                    else if (testX >= width)
                    {
                        testX = testX % width;
                    }
                    if (testY < 0)
                    {
                        testY = ((testY % height) + height) % height;
                    }
                    else if (testY >= height)
                    {
                        testY = testY % height;
                    }

                    if (field[testY][testX] != '#' && !plots.Contains(newSpot))
                    {
                        plots.Add(newSpot);

                        if (memory.ContainsKey(plot))
                        {
                            memory[plot].Add(newSpot);
                        }
                        else
                        {
                            memory.Add(plot, new HashSet<Point>() { newSpot });
                        }
                    }
                }
            }
            plotCounts.Add(i + 1, plots.Count);
        }
        return plotCounts;
    }

    static List<Point> directions = new List<Point>(){
        new Point(1, 0),
        new Point(-1, 0),
        new Point(0, 1),
        new Point(0, -1),
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