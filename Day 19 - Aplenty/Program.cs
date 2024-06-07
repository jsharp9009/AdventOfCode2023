using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace Aplenty;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var acceptNodes = ParseInput(input);
        var rules = GetRules(acceptNodes);
        var testObjects = GetTestObjects(input);
        var accepted = TestTheObjects(rules, testObjects);
        Console.WriteLine("Part 1: " + accepted.Sum(x => x.Total()));
        Console.WriteLine("Part 2: " + rules.Sum(CalcPotentials));
    }

    static List<TreeNode> ParseInput(string[] input)
    {
        var start = new TreeNode();
        var firstLine = input.First(c => c.StartsWith("in"));
        var accepted = new List<TreeNode>();
        ParseRecursive(firstLine, start, input, ref accepted);
        return accepted;
    }

    static List<TestObject> GetTestObjects(string[] input){
        var start = input.ToList().IndexOf("");
        var result = new List<TestObject>();
        for(int i = start +1; i < input.Length; i++){
            var parts = Regex.Match(input[i], "x=(\\d*),m=(\\d*),a=(\\d*),s=(\\d*)");
            result.Add(new TestObject(int.Parse(parts.Groups[1].Value), int.Parse(parts.Groups[2].Value), int.Parse(parts.Groups[3].Value), int.Parse(parts.Groups[4].Value)));
        }
        return result;
    }

    static TreeNode ParseRecursive(string line, TreeNode current, string[] input, ref List<TreeNode> acceptNodes)
    {
        var openBracketIndex = line.IndexOf('{');
        var colonIndex = line.IndexOf(':');
        var commaIndex = line.IndexOf(',');

        var condition = line.Substring(openBracketIndex + 1, colonIndex - openBracketIndex - 1);
        var trueCondition = line.Substring(colonIndex + 1, commaIndex - colonIndex - 1);
        var falseCondtion = line.Substring(commaIndex + 1).Replace("}", "");

        current.Condition = condition;
        if (trueCondition.Length == 1)
        {
            current.TrueResult = new TreeNode() { Parent = current, Condition = trueCondition, IsEnd = true };
            if (trueCondition == "A") {
                acceptNodes.Add(current.TrueResult);
            }
        }
        else
        {
            var nextline = input.First(c => c.StartsWith(trueCondition + "{"));
            var nextNode = new TreeNode() { Parent = current };
            nextNode = ParseRecursive(nextline, nextNode, input, ref acceptNodes);
            current.TrueResult = nextNode;
        }

        if (falseCondtion.Length == 1)
        {
            current.FalseResult = new TreeNode() { Parent = current, Condition = falseCondtion, IsEnd = true };
            if (falseCondtion == "A") {
                acceptNodes.Add(current.FalseResult);
            }
        }
        else if (falseCondtion.IndexOf(":") > 0)
        {
            var nextline = falseCondtion;
            var nextNode = new TreeNode() { Parent = current };
            nextNode = ParseRecursive(nextline, nextNode, input, ref acceptNodes);
            current.FalseResult = nextNode;
        }
        else
        {
            var nextline = input.First(c => c.StartsWith(falseCondtion + "{"));
            var nextNode = new TreeNode() { Parent = current };
            nextNode = ParseRecursive(nextline, nextNode, input, ref acceptNodes);
            current.FalseResult = nextNode;
        }

        return current;
    }

    private static List<Dictionary<string, int>> GetRules(List<TreeNode> acceptNodes)
    {
        List<Dictionary<string, int>> rules = new List<Dictionary<string, int>>();
        foreach (var node in acceptNodes)
        {
            var working = node;
            var parent = node.Parent;
            var rule = CreateRuleDictionary();
            while (parent != null)
            {
                var parts = Regex.Match(parent.Condition, "([a,s,m,x])([<,>])(\\d*)");
                var variable = parts.Groups[1].Value;
                var modifier = parts.Groups[2].Value;
                var value = int.Parse(parts.Groups[3].Value);
                var key = "";
                if (parent.TrueResult == working)
                {
                    switch (modifier)
                    {
                        case "<":
                            modifier = "Max";
                            key = variable + modifier;
                            if (value < rule[key]) rule[key] = value;
                            break;
                        case ">":
                            modifier = "Min";
                            key = variable + modifier;
                            if (value > rule[key]) rule[key] = value;
                            break;
                    }
                }
                else
                {
                    switch (modifier)
                    {
                        case ">":
                            modifier = "Max";
                            key = variable + modifier;
                            if (value < rule[key]) rule[key] = value + 1;
                            break;
                        case "<":
                            modifier = "Min";
                            key = variable + modifier;
                            if (value > rule[key]) rule[key] = value - 1;
                            break;
                    }
                }

                working = parent;
                parent = working.Parent;
            }
            rules.Add(rule);
        }
        return rules;
    }

    static Dictionary<string, int> CreateRuleDictionary()
    {
        return new Dictionary<string, int>{
            {"xMin", 0},
            {"xMax", 4001},
            {"mMin", 0},
            {"mMax", 4001},
            {"aMin", 0},
            {"aMax", 4001},
            {"sMin", 0},
            {"sMax", 4001}
        };
    }

    static List<TestObject> TestTheObjects(List<Dictionary<string, int>> rules, List<TestObject> tests){
        List<TestObject> result = new List<TestObject>();
        foreach(var test in tests){
            foreach(var rule in rules){
                if(test.x > rule["xMin"] && test.x < rule["xMax"]
                && test.m > rule["mMin"] && test.m < rule["mMax"]
                && test.a > rule["aMin"] && test.a < rule["aMax"]
                && test.s > rule["sMin"] && test.s < rule["sMax"]){
                    result.Add(test);
                    break;
                }
            }
        }
        return result;
    }

    static long CalcPotentials(Dictionary<string, int> rule){
        return (rule["xMax"] - rule["xMin"] - 1L) *
        (rule["mMax"] - rule["mMin"] - 1L) *
        (rule["aMax"] - rule["aMin"] - 1L) *
        (rule["sMax"]  -  rule["sMin"] - 1L);
    }
}

class TreeNode
{
    public TreeNode? Parent;
    public string Condition = "";
    public TreeNode? TrueResult;
    public TreeNode? FalseResult;
    public bool IsEnd = false;
}

record TestObject(int x, int m, int a, int s){
    public override string ToString()
    {
        return $"x={x},m={m},a={a},s={s}";
    }

    public int Total(){
        return x + m + a + s;
    }

}