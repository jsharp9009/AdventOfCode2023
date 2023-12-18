
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParabolicReflectorDish;

record Board(List<Point> RoundRocks, List<Point> CubeRocks, int Rows, int Collumns)
{
    public Board Shift(Point direction)
    {
        List<Point> roundRocks = new List<Point>();
        RoundRocks.OrderBy(r => r.row).ToList().ForEach(r => roundRocks.Add(new Point(r.row, r.column)));

        bool moved = false;
        do
        {
            moved = false;
            for (int i = 0; i < roundRocks.Count; i++)
            {
                while (true)
                {
                    var tryRoll = roundRocks[i] + direction;
                    if (tryRoll.row < 0 || tryRoll.row >= Rows
                    || tryRoll.column < 0 || tryRoll.column >= Collumns
                    || roundRocks.Contains(tryRoll) || CubeRocks.Contains(tryRoll))
                    {
                        break;
                    }

                    moved = true;
                    roundRocks[i] = tryRoll;
                }
            }
        } while (moved);
        return new Board(roundRocks, CubeRocks, Rows, Collumns);
    }

    public Board Cycle()
    {
        return Shift(new Point(-1, 0)).Shift(new Point(0, -1)).Shift(new Point(1, 0)).Shift(new Point(0, 1));
    }

    int hash = 0;
    public override int GetHashCode()
    {
        if (hash != 0) return hash;
        int hc = RoundRocks.Count;
        foreach (var val in RoundRocks)
        {
            hc = unchecked(hc * 314159 + val.GetHashCode());
        }
        hash = hc;
        return hc;
    }

    public void Print(){
        for(int row = 0; row < Rows; row++){
            for(int column = 0; column < Collumns; column++){
                var point = new Point(row, column);
                if(RoundRocks.Contains(point)){
                    Console.Write('O');
                }
                else if(CubeRocks.Contains(point)){
                    Console.Write('#');
                }
                else{
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

record Point(int row, int column)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.row + b.row, a.column + b.column);
    }

    public override int GetHashCode()
    {
        return (row * 100) + column;
    }
}
