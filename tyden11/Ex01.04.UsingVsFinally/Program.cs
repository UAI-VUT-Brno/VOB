Console.WriteLine("=== 01.04 – finally vs using ===\n");

Demo_UsingVsFinally();

// ──────────────────────────────────────────────
// 4. finally vs using
// ──────────────────────────────────────────────
static void Demo_UsingVsFinally()
{
    Console.WriteLine("--- finally vs using ---");
    // • using compiles to a try/finally that calls Dispose() — always prefer it for IDisposable objects.
    // • The using var declaration form (C# 8+) disposes at the end of the enclosing scope,
    //   not at the next closing brace.
    // • Reserve explicit finally for cleanup that Dispose() cannot express
    //   (e.g., resetting flags, releasing semaphores).

    // Verbose manual finally
    System.IO.MemoryStream? stream1 = null;
    try
    {
        stream1 = new System.IO.MemoryStream();
        stream1.WriteByte(42);
        Console.WriteLine($"[manual finally] Stream length: {stream1.Length}");
    }
    finally
    {
        stream1?.Dispose();
        Console.WriteLine("[manual finally] Stream disposed.");
    }

    // Preferred: using declaration (C# 8+)
    using var stream2 = new System.IO.MemoryStream();
    stream2.WriteByte(99);
    Console.WriteLine($"[using var]      Stream length: {stream2.Length}");
    // stream2.Dispose() called at end of enclosing scope

    Console.WriteLine();
}
