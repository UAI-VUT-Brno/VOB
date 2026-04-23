Console.WriteLine("=== 06.01 – Worker / BackgroundService pattern ===\n");

await Demo_WorkerServicePattern();

Console.WriteLine("=== Done ===");

// ──────────────────────────────────────────────
// 1. Worker / BackgroundService pattern
// ──────────────────────────────────────────────
static async Task Demo_WorkerServicePattern()
{
    Console.WriteLine("--- Worker / BackgroundService pattern ---");
    // • Background services run without a web request context, so exceptions require
    //   explicit top-level handling.
    // • A single unhandled exception escaping ExecuteAsync terminates the host — protect
    //   long-running loops with an internal try/catch.
    // • Always wrap host.RunAsync() in a try/finally to flush logs before the process exits.
    // • Log and continue on per-message errors — don't let one bad message kill the worker.

    using var cts = new CancellationTokenSource();

    var worker = new OrderWorker();

    // Let the worker process 3 messages then cancel
    var workerTask = worker.ExecuteAsync(cts.Token);
    await Task.Delay(180);
    cts.Cancel();

    try
    {
        await workerTask;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("  [host] Worker stopped gracefully.");
    }

    Console.WriteLine();
}

// ── Supporting types ──

class OrderWorker
{
    private int _messageCount;

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextMessageAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;  // graceful shutdown
            }
            catch (Exception ex)
            {
                // Log and continue — don't let one bad message kill the worker
                Console.WriteLine($"  [worker] Error processing message: {ex.Message} — continuing.");
                await Task.Delay(10, stoppingToken).ConfigureAwait(false);
            }
        }
    }

    private async Task ProcessNextMessageAsync(CancellationToken ct)
    {
        await Task.Delay(50, ct);
        _messageCount++;

        if (_messageCount == 2)
            throw new InvalidOperationException($"Message {_messageCount} was malformed.");

        Console.WriteLine($"  [worker] Processed message {_messageCount}.");
    }
}
