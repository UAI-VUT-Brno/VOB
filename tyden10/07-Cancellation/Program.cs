// Slide 10: CancellationToken a timeouty

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

var http = new HttpClient();

async Task<string> FetchWithTimeoutAsync(string url, CancellationToken cancellationToken)
{
    // Přidej vlastní timeout k existujícímu tokenu
    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    
    using var linked = CancellationTokenSource.CreateLinkedTokenSource(
        cancellationToken, timeoutCts.Token);

    // Token PROPAGUJ do volané metody – neztrácet ho!
    return await http.GetStringAsync(url, linked.Token);
}

// Ukázka 1: normální dokončení
Console.WriteLine("=== Normální dokončení ===");
using var cts1 = new CancellationTokenSource();
try
{
    string html = await FetchWithTimeoutAsync("https://www.seznam.cz", cts1.Token);
    Console.WriteLine($"Staženo {html.Length} znaků");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Zrušeno nebo timeout!");
}

// Ukázka 2: manuální okamžité zrušení
Console.WriteLine("=== Manuální zrušení ===");
using var cts2 = new CancellationTokenSource();
cts2.CancelAfter(TimeSpan.FromMilliseconds(1));
try
{
    await FetchWithTimeoutAsync("https://www.seznam.cz", cts2.Token);
}
catch (OperationCanceledException ex)
{
    Console.WriteLine($"Zachyceno: {ex.GetType().Name}: {ex.Message}");
}
