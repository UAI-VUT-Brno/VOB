Console.WriteLine("=== 05.03 – Blocking .Result wraps in AggregateException ===\n");

Demo_BlockingAntiPattern();

// ──────────────────────────────────────────────
// 3. Blocking anti-pattern (.Result / .Wait)
// ──────────────────────────────────────────────
static void Demo_BlockingAntiPattern()
{
    Console.WriteLine("--- Blocking .Result wraps in AggregateException ---");
    // • Blocking on a Task wraps its exceptions in AggregateException, requiring manual
    //   unwrapping — an immediate code smell.
    // • More critically, blocking can cause deadlocks in contexts that have a synchronization
    //   context (ASP.NET, WPF, WinForms).
    // • There is no legitimate reason to block on a Task in modern .NET — always await.

    // ⚠ BAD – demonstrated to show the AggregateException wrapping
    try
    {
        var _ = GetOrderAsync(Guid.NewGuid()).Result;
    }
    catch (AggregateException ae)
    {
        Console.WriteLine($"[.Result] AggregateException — InnerException: {ae.InnerException?.GetType().Name}: {ae.InnerException?.Message}");
    }

    Console.WriteLine("  → In real code always use: var order = await GetOrderAsync(id);");
    Console.WriteLine();
}

static async Task<Order> GetOrderAsync(Guid id)
{
    await Task.Delay(1);
    Order? order = null;
    return order ?? throw new OrderNotFoundException(id);
}

// ── Supporting types ──

record Order(Guid Id);

public sealed class OrderNotFoundException(Guid orderId)
    : Exception($"Order '{orderId}' was not found.")
{
    public Guid OrderId { get; } = orderId;
}
