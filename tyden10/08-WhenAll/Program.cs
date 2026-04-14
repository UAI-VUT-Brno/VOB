// Slide 11: Task.WhenAll – paralelní await vs sekvenční

using System.Diagnostics;

static Task<int> SimulateWork(string name, int ms) => Task.Run(async () =>
{
    await Task.Delay(ms);
    
    Console.WriteLine($"  {name} hotovo ({ms}ms)");
    
    return ms;
});

var sw = Stopwatch.StartNew();

// Sekvenční
Console.WriteLine("--- Sekvenční ---");

int r1 = await SimulateWork("A", 3000);
int r2 = await SimulateWork("B", 2000);
int r3 = await SimulateWork("C", 1000);

Console.WriteLine($"Sekvenční celkem: {sw.ElapsedMilliseconds}ms, součet: {r1 + r2 + r3}");

sw.Restart();

// Paralelní
Console.WriteLine("\n--- Paralelní ---");
Task<int>[] tasks =
[
    SimulateWork("X", 3000),
    SimulateWork("Y", 2000),
    SimulateWork("Z", 1000),
];
int[] results = await Task.WhenAll(tasks);
Console.WriteLine($"Paralelní celkem: {sw.ElapsedMilliseconds}ms, součet: {results.Sum()}");
