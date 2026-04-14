// Slide 13: Throttling – SemaphoreSlim.WaitAsync

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Velmi dulezity pattern pro omezeni soucasneho poctu operaci (napr. stahovani stranky, volani API) – pouziti SemaphoreSlim
const int maxConcurrency = 3;
using var throttler = new SemaphoreSlim(maxConcurrency);

var urls = Enumerable.Range(1, 10).Select(i => $"Stranka {i}").ToArray();

var tasks = new List<Task<string>>();

foreach (var url in urls)
{
    await throttler.WaitAsync();  // čekej asyncně na volné místo

    var capturedUrl = url;
    
    tasks.Add(Task.Run(async () =>
    {
        try
        {
            await Task.Delay(300); // simulace I/O
            
            Console.WriteLine($"  [{Thread.CurrentThread.ManagedThreadId:D2}] Staženo: {capturedUrl}");
            
            return capturedUrl;
        }
        finally
        {
            Console.WriteLine("  Uvolněn slot pro další operaci");  

            throttler.Release(); // uvolni slot pro dalšího
        }
    }));
}

string[] results = await Task.WhenAll(tasks);

Console.WriteLine($"Celkem staženo: {results.Length} polložek (max {maxConcurrency} současně)");
