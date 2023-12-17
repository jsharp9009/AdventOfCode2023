using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PointOfIncidence;

class Program
{

    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var boards = ParseInput(input);
        SolvePart1(boards);
        SolvePart2(boards);
    }

    static void SolvePart1(List<string[]> boards){
        int sum = 0;
        foreach (var board in boards)
        {
            var newBoard = Rotate(board);
            var horizontal = FindMirror(newBoard);
            var vertial = 0;
            if (horizontal == 0)
            {
                vertial = FindMirror(board);
            }
            sum += (vertial * 100) + horizontal;
        }
        Console.WriteLine("Part 1: " + sum);
    }

    static void SolvePart2(List<string[]> boards){
        int sum = 0;
        foreach (var board in boards)
        {
            var newBoard = Rotate(board);
            var horizontal = FindSmudgeMirror(newBoard);
            var vertial = 0;
            if (horizontal == 0)
            {
                vertial = FindSmudgeMirror(board);
            }
            
            sum += (vertial * 100) + horizontal;
        }
        Console.WriteLine("Part 2: " + sum);
    }

    static void PrintGrid(string[] grid)
    {
        foreach (var str in grid)
        {
            Console.WriteLine(str);
        }
    }

    static int FindMirror(string[] lines)
    {
        var firstIndex = lines.Length;
        while (firstIndex > 0)
        {
            firstIndex = lines.ToList().LastIndexOf(lines[0], firstIndex - 1);
            if (firstIndex > 0)
                if (CheckMirror(lines, 0, firstIndex))
                    return (int)Math.Ceiling(firstIndex / 2d);
        }

        var lastIndex = 0;
        while (lastIndex != -1)
        {
            lastIndex = lines.ToList().IndexOf(lines.Last(), lastIndex + 1);
            if (lastIndex != lines.Length - 1 && lastIndex >= 0)
                if (CheckMirror(lines, lastIndex, lines.Length - 1))
                    return ((lines.Length - lastIndex) / 2) + lastIndex;
        }

        return 0;
    }

    static int FindSmudgeMirror(string[] lines)
    {
        for(int i = lines.Length - 1; i > 0; i--){
            var dif = CountDifferences(lines[0], lines[i]);
            if(dif <= 1)
                if(dif == 1 && i == 1) return (int)Math.Ceiling(i / 2d);
                if(CheckSmudgeMirror(lines, 0, i))
                    return (int)Math.Ceiling(i / 2d);
        }

        for(int i = 0; i < lines.Length - 1; i++){
            var dif = CountDifferences(lines[i], lines[lines.Length - 1]);
            if (dif <= 1)
            {
                if (dif == 1 && i == lines.Length - 2) return ((lines.Length - i) / 2) + i;;
                if (CheckSmudgeMirror(lines, i, lines.Length - 1))
                    return ((lines.Length - i) / 2) + i;
            }
        }

        return 0;
    }

    static bool CheckMirror(string[] lines, int start, int end)
    {
        for (int s = start, e = end; s <= e; s++, e--)
        {
            if (s == e) return false;
            if (!lines[s].Equals(lines[e])) return false;
        }
        return true;
    }

    static bool CheckSmudgeMirror(string[] lines, int start, int end)
    {
        bool smudgeUsed = false;
        for (int s = start, e = end; s <= e; s++, e--)
        {
            if (s == e) return false;
            var dif = CountDifferences(lines[s], lines[e]);
            if (dif == 0) continue;
            else if (dif == 1 && !smudgeUsed){
                smudgeUsed = true;
            }
            else{
                return false;
            }
        }
        return smudgeUsed;
    }

    static string[] Rotate(string[] input)
    {
        var newLength = input[0].Length;
        var rotateed = new string[newLength];
        for (int i = 0; i < newLength; i++)
        {
            rotateed[i] = new string(input.Select(s => s[i]).ToArray());
        }
        return rotateed;
    }

    static int CountDifferences(string a, string b)
    {
        var dif = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) dif++;
        }
        return dif;
    }

    static List<string[]> ParseInput(string[] input)
    {
        List<string[]> boards = new List<string[]>();
        List<string> current = new List<string>();
        foreach (var line in input)
        {
            if (string.IsNullOrEmpty(line))
            {
                boards.Add(current.ToArray());
                current = new List<string>();
            }
            else
            {
                current.Add(line);
            }
        }
        boards.Add(current.ToArray());
        return boards;
    }
}
