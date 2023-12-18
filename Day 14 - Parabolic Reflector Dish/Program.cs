using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParabolicReflectorDish;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var board = ParseInput(input);
        SolvePart1(board);
        SolvePart2(board);
    }

    static void SolvePart1(Board board){
        var shifted = board.Shift(new Point(-1, 0));
        Console.WriteLine("Part 1: " + shifted.RoundRocks.Sum(r => shifted.Rows - r.row));
    }

    static void SolvePart2(Board board){
        var map = new Dictionary<int, Board>();
        var currentBoard = board;
        for(int i = 0; i < 1000000000; i++){
            if (map.ContainsKey(currentBoard.GetHashCode()))
             {
                currentBoard = map[currentBoard.GetHashCode()];
                continue;
            }

            var newBoard = currentBoard.Cycle();
            //newBoard.Print();
            map.Add(currentBoard.GetHashCode(), newBoard);
            currentBoard = newBoard;
        }
        Console.WriteLine("Part 2: " + currentBoard.RoundRocks.Sum(r => currentBoard.Rows - r.row));
    }

    static Board ParseInput(string[] input){
        var board = new Board(new List<Point>(), new List<Point>(), input.Length, input[0].Length);
        for(int row = 0; row < input.Length; row++){
            for(int column = 0; column < input[row].Length; column++){
                char c = input[row][column];
                if (c == 'O') board.RoundRocks.Add(new Point(row, column));
                else if (c == '#') board.CubeRocks.Add(new Point(row, column));
            }
        }
        return board;
    }
}



