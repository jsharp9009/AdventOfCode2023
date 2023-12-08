using System.Collections;
using System.Collections.Generic;

public class DirectionEnumerator : IEnumerator<int>
{
    string Directions;
    int currentIndex = 0;

    public DirectionEnumerator(string directions){
        Directions = directions;
    }

    public int Current => Directions[currentIndex % Directions.Length] == 'L' ? 0 : 1;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        currentIndex++;
        return true;
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}