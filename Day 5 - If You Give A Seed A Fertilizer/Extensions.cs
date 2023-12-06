using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions{
    public static long indexOf(this string[] input, string toFind, long start){
        for(long i = start; i < input.Length; i++){
            if (input[i].Equals(toFind, System.StringComparison.InvariantCultureIgnoreCase))
                return i;
        }
        return -1;
    }

    public static T[] SubArray<T>(this T[] array, long offset, long length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }

    public static IEnumerable<Range> Combine(this IEnumerable<Range> range){
        List<Range> ranges = new List<Range>();
        using (var it = range.GetEnumerator())
        {
            if (!it.MoveNext())
                return ranges;

            var item = it.Current;

            while (it.MoveNext())
                if (item.Overlap(it.Current))
                {
                    item = new Range(item.start, it.Current.end);
                }
                else
                {
                    ranges.Add(item);
                    item = it.Current;
                }

            ranges.Add(item);
        }
        return ranges.AsEnumerable<Range>();
    }
}