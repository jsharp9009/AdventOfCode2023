using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace SandSlabs;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var bricks = ParseInput(input);
        var resting = DropBrocks(bricks);
        var answer1 = resting.Count(b => CanDisintegrate(b));
        Console.WriteLine("Part 1: " + answer1);
        var answer2 = resting.Sum(b => DisitegrateChain(b));
        Console.WriteLine("Part 2: " + answer2);
    }

    static List<Brick> ParseInput(string[] input){
        List<Brick> bricks= new List<Brick>();
        foreach(string line in input){
            var parts = line.Split("~").SelectMany(c => c.Split(",")).Select(d => int.Parse(d)).ToArray();
            bricks.Add(new Brick(new Coordinate(parts[0], parts[1], parts[2]), new Coordinate(parts[3], parts[4], parts[5])));
        }
        return bricks;
    }

    static List<Brick> DropBrocks(List<Brick> bricks){
        var ordered = bricks.OrderBy(b => b.LowestPoint());
        foreach(var brick in ordered){
            if (brick.OnGround()) continue;

            var highest = 1;

            var lowerBricks = bricks.Where(l => l.LowestPoint() < brick.LowestPoint()).ToArray();

            if (lowerBricks.Length == 0) continue;

            foreach(var lowerBrick in lowerBricks){
                if(lowerBrick.IsUnder(brick)){
                    brick.Below.Add(lowerBrick);
                    lowerBrick.Above.Add(brick);
                    highest = Math.Max(highest, lowerBrick.HighestPoint() + 1);
                }
            }

            if(brick.LowestPoint() > highest){
                brick.Drop(brick.LowestPoint() - highest);
            }
        }
        return bricks.OrderBy(b => b.LowestPoint()).ToList();
    }

    static bool CanDisintegrate(Brick brick){
        foreach(var supported in brick.Supporting){
            if (supported.SupportedBy.Length == 1) return false;
        }
        return true;
    }

    static int DisitegrateChain(Brick brick){
        var que = new Queue<Brick>();
        que.Enqueue(brick);
        List<Brick> disintegrated = new List<Brick> { brick };

        while(que.Count > 0){
            var testBrick = que.Dequeue();
            foreach(var above in testBrick.Supporting){
                if (disintegrated.Contains(above)) continue;

                var supportsDisintegrated = above.SupportedBy.Count(b => disintegrated.Contains(b));
                if(supportsDisintegrated == above.SupportedBy.Count()){
                    disintegrated.Add(above);
                }
                que.Enqueue(above);
            }
        }
        return disintegrated.Count - 1;
    }
}

public record Coordinate(int x, int y, int z){
}

public record Range(int start, int end){
    public bool Overlap(Range compare){
        return !(end < compare.start || start > compare.end);
    }
}