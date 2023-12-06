using System.Collections.Generic;
using Microsoft.VisualBasic;

public class Almanac{
    public List<long> Seeds { get; set; } = new List<long>();
    public List<RuleSet> SeedToSoil { get; set; } = new List<RuleSet>();
    public List<RuleSet> SoilToFertilizer{ get; set; } = new List<RuleSet>();
    public List<RuleSet> FertilizerToWater{ get; set; } = new List<RuleSet>();
    public List<RuleSet> WaterToLight { get; set; } = new List<RuleSet>();
    public List<RuleSet> LightToTemp{ get; set; } = new List<RuleSet>();
    public List<RuleSet> TempToHumidity{ get; set; } = new List<RuleSet>();
    public List<RuleSet> HumidityToLocation{ get; set; } = new List<RuleSet>();
}

public record RuleSet(long DestinationStart, long SourceStart, long Length){

    public Range DestinationRage {get{ return new Range(DestinationStart, DestinationEnd); }}
    public Range SourceRange { get{ return new Range(SourceStart, SourceEnd); }}

    public long offset(){
        return DestinationStart - SourceStart;
    }

    public long DestinationEnd {get{
            return DestinationStart + Length;
    }}

    public long SourceEnd {get{
            return SourceStart + Length - 1;
    }}
};