using System;
using System.Collections.Generic;
using System.Linq;

public record Hand(string Cards, int Bet){
    public bool JokersWild { get; set; } = false;
    public static int GetCardValue(char card, bool jokersWild){
        return card switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => jokersWild ? 1 : 11,
            'T' => 10,
            _ => int.Parse(card.ToString()),
        };
    }

    public int GetHandValue(){
        var buckets = new Dictionary<char, int>();
        foreach(char c in Cards){
            if (buckets.ContainsKey(c))
                buckets[c]++;
            else
                buckets.Add(c, 1);
        }

        if(JokersWild){
            if (buckets.ContainsKey('J')){
                var wilds = buckets['J'];
                if (wilds == 5) return 6;
                buckets.Remove('J');
                var most = buckets.OrderByDescending(b => b.Value).First();
                buckets[most.Key] += wilds;
            }
        }

        if (buckets.Any(b => b.Value == 5))
            return 6;
        if (buckets.Any(b => b.Value == 4))
            return 5;
        if (buckets.Any(b => b.Value == 3)){
            if (buckets.Any(b => b.Value == 2))
                return 4;
            return 3;
        }
        if (buckets.Any(b => b.Value == 2))
        {
            if(buckets.Count(b => b.Value == 2) == 2){
                return 2;
            }
            return 1;
        }
        return 0;
    }   
}

public class HandComparer : IComparer<Hand>
{
    public int Compare(Hand? x, Hand? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var xScore = x.GetHandValue();
        var yScore = y.GetHandValue();

        if(xScore == yScore){
            for(int i = 0; i < 5; i++){
                if (x.Cards[i] != y.Cards[i])
                    return Hand.GetCardValue(x.Cards[i], x.JokersWild).CompareTo(Hand.GetCardValue(y.Cards[i], x.JokersWild));
            }
            return 0;
        }
        return xScore.CompareTo(yScore);
    }
}