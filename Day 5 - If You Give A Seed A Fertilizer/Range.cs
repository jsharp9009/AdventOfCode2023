using System;
using System.Collections;
using System.Collections.Generic;

public record Range(long start, long end){
    public bool Overlap(Range other)=> start <= other.end && other.start <= end;
    
    public Range Combine(Range other) => new Range(Math.Min(start, other.start), Math.Max(end, other.end));

    public IEnumerable<long> Enumerate(){
        for(long i = start; i <= end; i++){
            yield return i;
        }
    }
    
}