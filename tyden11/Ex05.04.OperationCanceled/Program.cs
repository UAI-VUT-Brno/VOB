Console.WriteLine("=== 05.04 – OperationCanceledException ===\n");

await Demo_OperationCanceled();

// ──────────────────────────────────────────────
// 4. OperationCanceledException / TaskCanceledException
// ──────────────────────────────────────────────
static async Task Demo_OperationCanceled()
{
    Console.WriteLine("--- OperationCanceledException ---");
    // • TaskCanceledException is a subclass of OperationCanceledException — catch the base
    //   class to handle both uniformly.
    // • Always re-throw after cleanup — suppressing cancellation breaks the cooperative
    //   cancellation chain across the entire call tree.
    // • Distinguish between intentional cancellation (ct.IsCancellationRequested) and a
    //   timeout or third-party-initiated cancellation.

    using var cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromMilliseconds(50));

    try
    {
        await ProcessAsync(cts.Token);
    }
    catch (OperationCanceledException ex)
    {
        Console.WriteLine($"[outer] Caught after re-throw: {ex.GetType().Name}");
    }

    Console.WriteLine();
}

static async Task ProcessAsync(CancellationToken ct)
{
    try
    {
        await Task.Delay(TimeSpan.FromSeconds(5), ct);
    }
    catch (OperationCanceledException) when (ct.IsCancellationRequested)
    {
        Console.WriteLine("  [ProcessAsync] Legitimate cancellation — cleanup done, re-throwing.");
        throw;  // always re-throw to preserve cooperative cancellation chain
    }
}
