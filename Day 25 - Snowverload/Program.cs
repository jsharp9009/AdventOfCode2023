using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Snowverload;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var graph = ParseInput(input);

        while(true){
            var testGraph = graph.Clone();
            while(testGraph.VerticesCount() > 2){
                var edge = testGraph.GetRandomEdge();
                testGraph.ContractEdge(edge.to, edge.from);
            }

            if (testGraph.EdgeCount(0) == 3)
            {
                Console.WriteLine(testGraph.GetAnswer());
                break;
            }
        }
    }

    static Graph ParseInput(string[] input){
        var graph = new Graph();
        foreach(var line in input){
            var parts = line.Split(":");
            var current = parts[0];
            var connecting = parts[1].Trim().Split(" ").Select(s => s.Trim());

            foreach(var connect in connecting){
                graph.Connect(current, connect);
            }
        }
        return graph;
    }
}

public class Graph : ICloneable{
    Dictionary<string, List<string>> vertices = new Dictionary<string, List<string>>();
    Dictionary<string, int> contractionCount = new Dictionary<string, int>();

    public int VerticesCount(){
        return vertices.Count;
    }

    public int EdgeCount(int position){
        return vertices[vertices.Keys.Skip(position).First()].Count;
    }

    public void Connect(string node1, string node2) { 
        if(vertices.ContainsKey(node1)){
            if(!vertices[node1].Contains(node2)){
                vertices[node1].Add(node2);
            }
        }
        else{
            vertices.Add(node1, new List<string>(){node2});
        }

        if(vertices.ContainsKey(node2)){
            if(!vertices[node2].Contains(node1)){
                vertices[node2].Add(node1);
            }
        }
        else{
            vertices.Add(node2, new List<string>(){node1});
        }

        if(!contractionCount.Keys.Contains(node1)) 
            contractionCount.Add(node1, 1);
        if(!contractionCount.Keys.Contains(node2)) 
            contractionCount.Add(node2, 1);
    }

    public (string to, string from) GetRandomEdge(){
        var random = new Random();
        var to = random.Next(vertices.Count);
        var toNode = vertices.Keys.Skip(to - 1).First();
        var from = random.Next(vertices[toNode].Count);
        var fromNode = vertices[toNode][from];
        if (fromNode == toNode) return GetRandomEdge();
        return (toNode, fromNode);
    }

    public void ContractEdge(string to, string from){
        //Console.WriteLine(from + " -> " + to);
        foreach(var vert in vertices[from]){
            vertices[vert].Remove(from);
            if (vert == to) continue;
            vertices[to].Add(vert);
            vertices[vert].Add(to);
        }
        
        //vertices[to].AddRange(vertices[from].Where(c => c != to));
        //vertices[to].Remove(from);
        vertices.Remove(from);

        contractionCount[to] += contractionCount[from];
    }    
    
    public int GetAnswer(){
        var answer = 1;
        foreach(var v in vertices.Keys){
            answer *= contractionCount[v];
        }
        return answer;
    }

    object ICloneable.Clone(){
        return Clone();
    }

    public Graph Clone()
    {
         Graph graph = new Graph();
        graph.vertices = vertices.ToDictionary(e => e.Key, e => new List<string>(vertices[e.Key]));
        graph.contractionCount = new Dictionary<string, int>(contractionCount);
        return graph;
    }
}