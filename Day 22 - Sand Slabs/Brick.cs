using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SandSlabs;
public class Brick{
    public Coordinate Position1;
    public Coordinate Position2;
    public List<Brick> Below = new List<Brick>();
    public List<Brick> Above = new List<Brick>();

    private Brick[]? supporting;
    public Brick[] Supporting{
        get{ 
            if(supporting == null){
                supporting = Above.Where(b => IsLevelBelow(b)).ToArray();
            }
            return supporting;
         }
    }

private Brick[]? supportedBy;
    public Brick[] SupportedBy{
        get{ 
            if(supportedBy == null){
                supportedBy = Below.Where(b => b.IsLevelBelow(this)).ToArray();
            }
            return supportedBy;
         }
    }

    public Brick(Coordinate Position1, Coordinate Position2) {
        this.Position1 = Position1;
        this.Position2 = Position2;
    }

    public int LowestPoint(){
        return Math.Min(this.Position1.z, this.Position2.z);
    }

    public int HighestPoint(){
        return Math.Max(this.Position1.z, this.Position2.z);
    }

    public bool OnGround(){
        return this.LowestPoint() == 1;
    }

    public bool IsUnder(Brick other){
        var thisXRange = new Range(Math.Min(this.Position1.x, this.Position2.x), Math.Max(this.Position1.x, this.Position2.x));
        var otherXRange = new Range(Math.Min(other.Position1.x, other.Position2.x), Math.Max(other.Position1.x, other.Position2.x));

        var thisYRange = new Range(Math.Min(this.Position1.y, this.Position2.y), Math.Max(this.Position1.y, this.Position2.y));
        var otherYRange = new Range(Math.Min(other.Position1.y, other.Position2.y), Math.Max(other.Position1.y, other.Position2.y));

        return thisXRange.Overlap(otherXRange) && thisYRange.Overlap(otherYRange);
    }

    public bool IsLevelBelow(Brick other){
        return this.HighestPoint() == other.LowestPoint() - 1;
    }

    public void Drop(int amount = 1){
        this.Position1 = new Coordinate(Position1.x, Position1.y, Position1.z - amount);
        this.Position2 = new Coordinate(Position2.x, Position2.y, Position2.z - amount);
    }
}