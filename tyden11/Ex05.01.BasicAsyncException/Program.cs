Console.WriteLine("=== 05.01 – Async exception propagation ===\n");

await Demo_BasicAsyncException();

// ──────────────────────────────────────────────
// 1. Exception captured in Task, re-thrown at await
// ──────────────────────────────────────────────
static async Task Demo_BasicAsyncException()
{
    Console.WriteLine("--- Async exception propagation ---");
    // • In async methods, exceptions are not thrown immediately — they are captured in the
    //   returned Task and re-thrown at the await point.
    // • A try/catch around an await expression works as expected; wrapping the call without
    //   await does not intercept the exception.
    // • The captured exception preserves its original stack trace, just as with synchronous throw.

    try
    {
        var order = await GetOrderAsync(Guid.NewGuid());
        Console.WriteLine($"Order: {order}");
    }
    catch (OrderNotFoundException ex)
    {
        Console.WriteLine($"[await catch] {ex.Message}");
    }

    Console.WriteLine();
}

static async Task<Order> GetOrderAsync(Guid id)
{
    await Task.Delay(1);   // simulate async work
    Order? order = null;   // simulate not-found
    return order ?? throw new OrderNotFoundException(id);
}

// ── Supporting types ──

record Order(Guid Id);

public sealed class OrderNotFoundException(Guid orderId)
    : Exception($"Order '{orderId}' was not found.")
{
    public Guid OrderId { get; } = orderId;
}
