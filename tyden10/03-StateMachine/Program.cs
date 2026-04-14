// Slide 05: async/await – stavový stroj kompilátoru

using System;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("=== Demonstrace stavového stroje ===");
await DemoAsync();
Console.WriteLine("Volající pokračuje po dokončení DemoAsync.");

async Task DemoAsync()
{
    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Před await");

    // Kompilátor zde 'rozřízne' metodu.
    // Řízení se OKAMŽITĚ vrátí volajícímu, metoda 'pokračuje' po 100ms.
    await Task.Delay(100);

    // Toto je POKRAČOVÁNÍ – spustí se jako callback po 100ms
    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Po await");
    Console.WriteLine("Stavový stroj přešel do finálního stavu (RanToCompletion).");
}
