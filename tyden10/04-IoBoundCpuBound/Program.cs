// Slide 06: I/O-bound vs CPU-bound

using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

var http = new HttpClient();

// ✅ I/O-bound: nativní async API – žádný Task.Run!
async Task<int> GetPageLengthAsync(string url)
{
    string html = await http.GetStringAsync(url);   // skutečné async I/O
    return html.Length;
}

// ✅ CPU-bound: offload na thread pool přes Task.Run
async Task<string> HashAsync(string input)
{
    return await Task.Run(() =>
    {
        using var sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    });
}

int len = await GetPageLengthAsync("https://example.com");
Console.WriteLine($"Délka stránky: {len} znaků");

string hash = await HashAsync("hello");
Console.WriteLine($"SHA256: {hash[..16]}…");
