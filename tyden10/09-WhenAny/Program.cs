// Slide 12: Task.WhenAny – race a timeout pattern

using System;
using System.Net.Http;
using System.Threading.Tasks;

var http = new HttpClient();

// ✅ Klasický pattern pro timeout bez CancellationToken – pomocí Task.WhenAny
// Nevýhoda: fetchTask není zrušen, ale timeout nám umožní reagovat dříve
// TROCHU ANTIPATTERN!!!!

// Pattern: WhenAny jako timeout 
async Task<string?> FetchWithWhenAnyTimeoutAsync(string url, int timeoutMs)
{
    Task<string> fetchTask = http.GetStringAsync(url);
    Task timeoutTask = Task.Delay(timeoutMs);

    Task winner = await Task.WhenAny(fetchTask, timeoutTask);

    if (winner == timeoutTask)
    {
        Console.WriteLine("⏰ Timeout – operace trvá příliš dlouho");
        return null;
    }

    // fetchTask je hotový; await ho pro propagaci případné výjimky
    return await fetchTask;
}

// Ukázka 1: stihne se během 5s
Console.WriteLine("=== Fetch s 5s timeoutem ===");
string? result = await FetchWithWhenAnyTimeoutAsync("https://www.seznam.cz", 5000);
Console.WriteLine(result != null ? $"Délka: {result.Length}" : "Timeout");

// Ukázka 2: timeout 1ms = okamžitě vyprší
Console.WriteLine("\n=== Fetch s 1ms timeoutem ===");
string? result2 = await FetchWithWhenAnyTimeoutAsync("https://www.seznam.cz", 1);
Console.WriteLine(result2 != null ? $"Délka: {result2.Length}" : "Timeout");
