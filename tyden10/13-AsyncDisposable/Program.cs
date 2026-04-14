// Slide 17: IAsyncDisposable + await using
// V C# top-level statements musí předcházet deklaracím tříd v souboru.
// Třída AsyncLogger je definována níže (za top-level kódem) – C# ji vidí celý soubor.

using System;
using System.IO;
using System.Threading.Tasks;

// ✅ await using: DisposeAsync zavolána automaticky i při výjimce
string logPath = Path.GetTempFileName();

await using (var logger = new AsyncLogger(logPath))
{
    await logger.LogAsync("Aplikace spuštěna");
    await logger.LogAsync("Zpracování dat...");
}  // ← DisposeAsync() zavolána zde automaticky (i při výjimce)

string content = await File.ReadAllTextAsync(logPath);

Console.WriteLine("\nObsah logu:");

Console.Write(content);

File.Delete(logPath);

// ---- Definice třídy (musí být za top-level kódem) ----

public sealed class AsyncLogger : IAsyncDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    public AsyncLogger(string path)
    {
        _writer = new StreamWriter(path, append: true);
        Console.WriteLine("Logger otevřen");
    }

    public async Task LogAsync(string message)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        await _writer.WriteLineAsync($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        await _writer.FlushAsync();
        await _writer.DisposeAsync();
        
        Console.WriteLine("Logger uvolněn (async)");
    }
}
