Console.WriteLine("=== 05.02 – AggregateException (Task.WhenAll) ===\n");

await Demo_AggregateException();

// ──────────────────────────────────────────────
// 2. AggregateException from Task.WhenAll
// ──────────────────────────────────────────────
static async Task Demo_AggregateException()
{
    Console.WriteLine("--- AggregateException (Task.WhenAll) ---");
    // • AggregateException wraps multiple concurrent exceptions into a single object;
    //   Flatten() unwraps nested aggregates.
    // • When you await Task.WhenAll(...), only the first exception is re-thrown automatically —
    //   inspect each faulted task for the full picture.
    // • ae.Handle(predicate) allows selective handling: return true to mark an exception as
    //   handled, false to re-throw the remainder.

    var tasks = new[]
    {
        FetchAsync("A", fail: false),
        FetchAsync("B", fail: true),
        FetchAsync("C", fail: true),
    };

    try
    {
        await Task.WhenAll(tasks);
    }
    catch
    {
        // Only the first exception is re-thrown by await — inspect each faulted task
        foreach (var task in tasks.Where(t => t.IsFaulted))
        {
            foreach (var inner in task.Exception!.InnerExceptions)
                Console.WriteLine($"  Task failed: {inner.Message}");
        }
    }

    Console.WriteLine();
}

static async Task FetchAsync(string label, bool fail)
{
    await Task.Delay(1);
    if (fail)
        throw new InvalidOperationException($"Fetch '{label}' failed.");
    Console.WriteLine($"  Fetch '{label}' succeeded.");
}
