using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CosmicExpansion;

class Program
{
    // Too Low 9653739
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt").Select(c => c.ToArray()).ToArray();
        var galaxies = FindGalaxies(input);
        (List<int> expandRows, List<int> expandColumsn) = GetExpandexSections(input);
        
        //Part 1
        var expandedGalaxies = ExpandGalaxies(galaxies, expandRows, expandColumsn, 1);
        var distances = GetDistances(expandedGalaxies);
        Console.WriteLine("Part 1: " + distances.Sum());

        //Part 2
        expandedGalaxies = ExpandGalaxies(galaxies, expandRows, expandColumsn, 999999);
        distances = GetDistances(expandedGalaxies);
        Console.WriteLine("Part 2: " + distances.Sum());
    }

    
    static (List<int> rows, List<int> colums) GetExpandexSections(char[][] input)
    {
        List<int> expandingRows = new List<int>();
        for (int i = 0; i < input.Count(); i++)
        {
            if (input[i].All(c => c == '.'))
                expandingRows.Add(i);
        }

        List<int> expandingColumsn = new List<int>();
        for (int i = 0; i < input[0].Length; i++)
        {
            if (input.All(l => l[i] == '.'))
                expandingColumsn.Add(i);
        }

        return (expandingRows, expandingColumsn);
    }

    static List<Point> FindGalaxies(char[][] universe)
    {
        var galaxies = new List<Point>();
        for (int row = 0; row < universe.Length; row++)
        {
            for (int column = 0; column < universe[row].Length; column++)
            {
                if (universe[row][column] == '#')
                {
                    galaxies.Add(new Point(column, row));
                }
            }
        }
        return galaxies;
    }

    static List<Point> ExpandGalaxies(List<Point> galaxies, List<int> expandRows, List<int> expandColumns, int expansion)
    {
        var newGalaxies = new List<Point>();
        foreach (var galaxy in galaxies)
        {
            var pastRows = expandRows.Count(c => c < galaxy.row);
            var pastColumns = expandColumns.Count(c => c < galaxy.column);
            newGalaxies.Add(new Point(galaxy.column + (pastColumns * expansion), galaxy.row + (pastRows * expansion)));
        }
        return newGalaxies;
    }

    static List<long> GetDistances(List<Point> galaxies)
    {
        var distances = new List<long>();
        for (int i = 0; i < galaxies.Count; i++)
        {
            for (int n = i + 1; n < galaxies.Count; n++)
            {
                distances.Add(Math.Abs(galaxies[i].column - galaxies[n].column) + Math.Abs(galaxies[i].row - galaxies[n].row));
            }
        }
        return distances;
    }
}

record Point(int column, int row);