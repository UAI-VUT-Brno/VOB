// Slide 18: Anti-patternsy

using System;
using System.Threading.Tasks;

// ✅ SPRÁVNĚ: vrátit Task, chýby ošetřit
async Task SafeBackgroundAsync()
{
    try
    {
        await Task.Delay(50);
        Console.WriteLine("✅ Safe background: dokončeno");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Logováno: {ex.Message}");
    }
}

// ❌ ŠPATNĚ: async void – volající nemá Task, výjimka se může ztratit
// POZNÁMKA: V konzoli .NET 8 může výjimka skončit jako UnhandledException;
//          v non-console aplikaci by tiše zmizela nebo shodila proces.
static async void FireAndForgetBad()
{
    await Task.Delay(50);
    Console.WriteLine("❌ async void: běžím, ale volající nemá žádný Task");
}

// ❌ ŠPATNĚ: vyvolání fire-and-forget (bez await, bez ošetření)
FireAndForgetBad();   // <-- kompilátor varuje: CS4014 (neawaitovaný Task)
Console.WriteLine("Po FireAndForgetBad – výjimka mohla zmizet!");

// ✅ SPRÁVNĚ: awaitable Task se ošetří
await SafeBackgroundAsync();

// ❌ ŠPATNĚ: sync-over-async (v konzoli nedeadlockuje, ale plytvá vlákny)
Console.WriteLine("\nSync-over-async (přesto špatný vzor):");
string bad = Task.Run(async () => { await Task.Delay(10); return "hotovo"; }).Result;
Console.WriteLine($"  Výsledek: {bad} (použito .Result – nedelej to!)");

// ✅ SPRÁVNĚ: await
string good = await Task.Run(async () => { await Task.Delay(10); return "hotovo"; });
Console.WriteLine($"  Výsledek: {good} (použito await – správně)");
