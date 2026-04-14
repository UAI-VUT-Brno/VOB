// Slide 08: ConfigureAwait(false)

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

var http = new HttpClient();

// ✅ Dobře v KNIHOVNÍM kódu – nepotřebujeme kontext volající aplikace
async Task<int> LibraryMethodAsync(string url)
{
    // ConfigureAwait(false): pokračování může běžet na libovolném vlákně
    string html = await http.GetStringAsync(url).ConfigureAwait(false);
    // Zde NESMÍME přistupovat k UI prvkům (nemáme garantovaný UI thread)
    return html.Length;
}

Console.WriteLine($"Před await – thread: {Thread.CurrentThread.ManagedThreadId}");
int len = await LibraryMethodAsync("https://example.com");
Console.WriteLine($"Po await  – thread: {Thread.CurrentThread.ManagedThreadId}, len: {len}");
Console.WriteLine("(V konzoli bez SynchronizationContext se threadId může lišit)");
