// Slide 09: Výjimky – await vs .Wait()/.Result

using System;
using System.Threading.Tasks;

// ✅ SPRÁVNĚ: await + try/catch → normální výjimka (unwrapped)
async Task GoodAsync()
{
    try
    {
        await Task.Run(() => throw new InvalidOperationException("Boom!"));
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"✅ Chyceno normálně: {ex.Message}");
    }
}

// ❌ ŠPATNĚ: .Wait() → AggregateException wrapper
void BadBlocking()
{
    try
    {
        Task.Run(() => throw new InvalidOperationException("Boom!")).Wait();
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"✅ Chyceno normálně: {ex.Message}");
    }
    catch (AggregateException agg)
    {
        Console.WriteLine($"❌ AggregateException: {agg.InnerExceptions[0].Message}");
    }
}

// ⚠️ WhenAll: výjimky ze VŠECH tasků jsou agregovány
async Task WhenAllExceptions()
{
    Task[] tasks =
    [
        Task.Run(() => throw new Exception("Chyba 1")),
        Task.Run(() => throw new Exception("Chyba 2")),
    ];

    Task combined = Task.WhenAll(tasks);
    try
    {
        await combined;
    }
    catch
    {
        Console.WriteLine("WhenAll selhal – všechny výjimky:");
        foreach (var ex in combined.Exception!.InnerExceptions)
            Console.WriteLine($"  • {ex.Message}");
    }
}

await GoodAsync();
BadBlocking();
await WhenAllExceptions();
