Console.WriteLine("=== 03.01 – When to throw ===\n");

Demo_WhenToThrow();

// ──────────────────────────────────────────────
// 1. When to throw — precondition & state violations
// ──────────────────────────────────────────────
static void Demo_WhenToThrow()
{
    Console.WriteLine("--- When to throw ---");
    // • Throw when the caller has violated a contract or an operation cannot succeed
    //   in the current state.
    // • Throwing establishes a contract: callers know certain preconditions must be met.
    // • Always prefer specific exception types over Exception — they communicate intent
    //   and enable targeted catching.
    // • Never throw for conditions that are part of normal, expected program flow.

    // Precondition violation
    try
    {
        CalculateDiscount(null!, 10);
    }
    catch (ArgumentNullException ex)
    {
        Console.WriteLine($"[precondition] {ex.Message}");
    }

    try
    {
        CalculateDiscount(new Order(Guid.NewGuid(), 200m), -5);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"[out of range] {ex.Message}");
    }

    var result = CalculateDiscount(new Order(Guid.NewGuid(), 200m), 10);
    Console.WriteLine($"Discount result: {result:C}");

    // State violation
    var processor = new OrderProcessor();
    processor.Dispose();
    try
    {
        processor.StartProcessing();
    }
    catch (ObjectDisposedException ex)
    {
        Console.WriteLine($"[state] {ex.Message}");
    }

    Console.WriteLine();
}

static decimal CalculateDiscount(Order order, decimal percentage)
{
    ArgumentNullException.ThrowIfNull(order);
    ArgumentOutOfRangeException.ThrowIfNegative(percentage);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(percentage, 100);
    return order.Total * (percentage / 100);
}

// ── Supporting types ──

record Order(Guid Id, decimal Total);

class OrderProcessor : IDisposable
{
    private bool _isDisposed;
    private bool _isRunning;

    public void StartProcessing()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(OrderProcessor));
        if (_isRunning)
            throw new InvalidOperationException("Processing is already running.");
        _isRunning = true;
        Console.WriteLine("Processing started.");
    }

    public void Dispose()
    {
        _isDisposed = true;
        _isRunning = false;
    }
}
