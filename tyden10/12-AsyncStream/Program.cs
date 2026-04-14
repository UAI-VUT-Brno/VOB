// Slide 16: IAsyncEnumerable<T> + await foreach

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

// ✅ IAsyncEnumerable<T> – asynchronní generátor, který umožňuje streamovat data postupně (např. pro načítání z DB, API, nebo generování dat)
// ✅ Generátor: streamuje čísla postupně (simuluje DB stránkování)
static async IAsyncEnumerable<int> GenerateNumbersAsync(
    int count,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    for (int i = 1; i <= count; i++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await Task.Delay(50, cancellationToken); // simulace async I/O
        
        Console.WriteLine($"  Generuji: {i}");
        
        yield return i;   // vrátí hodnotu a POZASTAVÍ generátor
    }
}

// ✅ Praktický příklad: stránkované načítání z 'databáze'
static async IAsyncEnumerable<string> GetUsersAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    string[] page1 = ["Alice", "Bob"];
    string[] page2 = ["Charlie", "Diana"];

    foreach (var name in page1) { await Task.Delay(30, ct); yield return name; }
    foreach (var name in page2) { await Task.Delay(30, ct); yield return name; }
}

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

Console.WriteLine("=== Async Stream ===");
await foreach (int number in GenerateNumbersAsync(5, cts.Token))
{
    Console.WriteLine($"  Zpracovávám: {number}");
}

Console.WriteLine("\n=== Stránkovaní uživatelé ===");

await foreach (var user in GetUsersAsync())
{
    Console.WriteLine($"  Uživatel: {user}");
}
