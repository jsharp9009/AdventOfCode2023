using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PulsePropagation;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var modules = ParseInput(input);
        SolvePart1(modules);
        SolvePart2(modules);
    }

    static void SolvePart1(Dictionary<string, Module> modules)
    {
        var presses = 0;
        var highPing = 0;
        var lowPing = 0;
        while (presses < 1000)
        {
            var res = PressButton(modules);
            highPing += res.High;
            lowPing += res.Low;
            presses++;
        }
        Console.WriteLine("Part 1: " + (highPing) * (lowPing));
    }

    static void SolvePart2(Dictionary<string, Module> modules)
    {
        var rxCaller = modules.First(m => m.Value.Listeners.Contains("rx")).Value;
        var callers = ((Conjunction)rxCaller).Callers;
        var lcm = 1L;
        foreach (var caller in callers)
        {
            Reset(modules);
            lcm = LCM(lcm, FindPressesSendToHigh(caller, modules));
        }
        Console.WriteLine("Part 2: " + lcm);
    }

    static (int High, int Low) PressButton(Dictionary<string, Module> modules)
    {
        var pings = new Queue<PingRequest>();
        pings.Enqueue(new PingRequest("", false, "broadcaster"));
        int highPing = 0;
        int lowPing = 1;

        while (pings.TryDequeue(out var ping))
        {
            if (!modules.ContainsKey(ping.To)) continue;
            var to = modules[ping.To];
            foreach (var p in to.HandlePing(ping))
            {
                if (p.HighLow) highPing++;
                else lowPing++;

                pings.Enqueue(p);
            }
        }
        return (highPing, lowPing);
    }

    static Dictionary<string, Module> ParseInput(string[] input)
    {
        Dictionary<string, Module> modules = new Dictionary<string, Module>();
        foreach (string line in input)
        {
            var parts = line.Split(" -> ");
            if (parts[0] == "broadcaster")
            {
                modules.Add(parts[0], new Broadcast(parts[0], parts[1].Split(",").Select(x => x.Trim()).ToList()));
            }
            else if (parts[0][0] == '%')
            {
                modules.Add(parts[0].Substring(1), new FlipFlop(parts[0].Substring(1), parts[1].Split(",").Select(x => x.Trim()).ToList()));
            }
            else if (parts[0][0] == '&')
            {
                var name = parts[0].Substring(1);
                var callers = input.Where(x => x.IndexOf(name) > 2).Select(x => x.Split(" ")[0].Substring(1));
                modules.Add(name, new Conjunction(name, parts[1].Split(",").Select(x => x.Trim()).ToList(), callers.ToList()));
            }
        }
        return modules;
    }


    public static Dictionary<string, Module> Reset(Dictionary<string, Module> modules)
    {
        foreach (var key in modules.Keys)
        {
            modules[key].Reset();
        }
        return modules;
    }

    public static int FindPressesSendToHigh(string moduleName, Dictionary<string, Module> modules)
    {
        var presses = 0;
        var highPing = 0;
        var lowPing = 0;
        while (true)
        {
            var pings = new Queue<PingRequest>();
            pings.Enqueue(new PingRequest("", false, "broadcaster"));

            while (pings.TryDequeue(out var ping))
            {
                if (ping.From == moduleName && ping.HighLow)
                {
                    return presses + 1;
                }
                if (!modules.ContainsKey(ping.To)) continue;
                var to = modules[ping.To];
                foreach (var p in to.HandlePing(ping))
                {
                    if (p.HighLow) highPing++;
                    else lowPing++;

                    pings.Enqueue(p);
                }
            }
            presses++;
        }
    }

    static long GCF(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long LCM(long a, long b)
    {
        return (a / GCF(a, b)) * b;
    }
}

public abstract class Module
{

    public string Name { get; set; }
    public List<string> Listeners = new List<string>();
    public abstract IEnumerable<PingRequest> HandlePing(PingRequest ping);
    public abstract void Reset();
    public Module(string name, List<string> listeners)
    {
        Name = name;
        Listeners = listeners;
    }
}

public record PingRequest(string From, bool HighLow, string To) { }

public class FlipFlop : Module
{
    private bool state = false;

    public FlipFlop(string name, List<string> listeners) : base(name, listeners)
    {
    }

    public override IEnumerable<PingRequest> HandlePing(PingRequest ping)
    {
        if (ping.HighLow == false)
        {
            state = !state;
            foreach (var l in this.Listeners)
            {
                yield return new PingRequest(Name, state, l);
            }
        }
    }

    public override void Reset()
    {
        state = false;
    }
}

public class Conjunction : Module
{
    Dictionary<string, bool> states = new Dictionary<string, bool>();
    public List<string> Callers;

    public Conjunction(string name, List<string> listeners, List<string> callers) : base(name, listeners)
    {
        foreach (var l in callers)
        {
            states.Add(l, false);
        }
        this.Callers = callers;
    }

    public override IEnumerable<PingRequest> HandlePing(PingRequest ping)
    {
        states[ping.From] = ping.HighLow;
        var pulse = !states.All(c => c.Value);
        foreach (var l in this.Listeners)
        {
            yield return new PingRequest(Name, pulse, l);
        }
    }

    public override void Reset()
    {
        foreach (var key in states.Keys)
        {
            states[key] = false;
        }
    }
}

public class Broadcast : Module
{
    public Broadcast(string name, List<string> listeners) : base(name, listeners)
    {
    }

    public override IEnumerable<PingRequest> HandlePing(PingRequest ping)
    {
        foreach (var l in this.Listeners)
        {
            yield return new PingRequest(Name, ping.HighLow, l);
        }
    }

    public override void Reset()
    {
    }
}