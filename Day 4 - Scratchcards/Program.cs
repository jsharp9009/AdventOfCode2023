using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scratchcards;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var cards = ParseInput(input);
        SolvePart1(cards);
        SolvePart2(cards);
    }

    static void SolvePart1(List<ScratchCard> input){
        var totalPoints = 0;
        foreach(var line in input){
            totalPoints += line.GetPoints();
        }
        Console.WriteLine("Part 1: " + totalPoints);
    }

    static void SolvePart2(List<ScratchCard> cards) {
        Dictionary<ScratchCard, int> cardTotals = new Dictionary<ScratchCard, int>();
        cards.ForEach(c => cardTotals.Add(c, 1));

        for (int i = 0; i < cards.Count; i++) {
            var cardCount = cardTotals[cards[i]];
            var winners = cards[i].CountWinnings();

            for (int n = 0; n < winners; n++){
                cardTotals[cards[i + n + 1]] += cardCount;
            }
        }

        Console.WriteLine("Part 2:" + cardTotals.Values.Sum());
    }

    static List<ScratchCard> ParseInput(string[] input){
        var cards = new List<ScratchCard>();
        foreach (var line in input)
        {
            cards.Add(new ScratchCard(line));
        }
        return cards;
    }
}

public record ScratchCard(List<int> Winning, List<int> YourNumbers){
    public ScratchCard(string input):this([], []){
        var index1 = input.IndexOf(":");
        var indexPipe = input.IndexOf("|");

        var winningNums = input.Substring(index1 + 2, indexPipe - index1 - 2);
        var yourNums = input.Substring(indexPipe + 2);
        
        winningNums.Trim().Split(" ").ToList().ForEach(c => {
            if (!string.IsNullOrEmpty(c))
                Winning.Add(int.Parse(c));
        });
        yourNums.Trim().Split(" ").ToList().ForEach(c => {
            if (!string.IsNullOrEmpty(c))
                YourNumbers.Add(int.Parse(c));
        });
    }

    public int GetPoints(){
        var winning = CountWinnings();
        var points = 0;
        for(int i = 0; i < winning; i++){
            if(points == 0) {
                points++;
                continue;
            }
            points = points * 2;
        }
        return points;
    }

    public int CountWinnings(){
        return Winning.Intersect(YourNumbers).Count();
    }
}
