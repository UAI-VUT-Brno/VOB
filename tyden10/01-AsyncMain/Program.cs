// Slide 03: První async program – async Main
// Spusť: dotnet run --project examples/01-AsyncMain

using System;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Start");

await Task.Delay(500);   // neblokuje vlákno – klíčový rozdíl od Thread.Sleep

Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Po await");
Console.WriteLine("(ThreadId se může lišit – pokračování na jiném vlákně z pool)");

// Zkus odkomentovat a porovnej:
// Thread.Sleep(500);  // blokuje vlákno, threadId stejný
