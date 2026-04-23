Console.WriteLine("=== 02.01 – Built-in exception types ===\n");

Demo_BuiltinExceptions();

// ──────────────────────────────────────────────
// 1. Choosing the right built-in exception
// ──────────────────────────────────────────────
static void Demo_BuiltinExceptions()
{
    Console.WriteLine("--- Built-in exception types ---");
    // • Prefer the most specific built-in exception type before defining a custom one.
    // • ArgumentException and its subclasses cover caller-side violations;
    //   InvalidOperationException covers object-state violations.
    // • Never throw Exception or SystemException directly — they carry no semantic meaning for the caller.
    // • NullReferenceException and IndexOutOfRangeException signal bugs — fix the code, don't catch them.

    // ArgumentNullException.ThrowIfNull (.NET 6+)
    try
    {
        Order? order = null;
        ArgumentNullException.ThrowIfNull(order);
    }
    catch (ArgumentNullException ex)
    {
        Console.WriteLine($"[ArgumentNullException] {ex.Message}");
    }

    // ArgumentOutOfRangeException
    try
    {
        SetTimeout(-5);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"[ArgumentOutOfRangeException] {ex.Message}");
    }

    // InvalidOperationException
    try
    {
        var processor = new OrderProcessor();
        processor.Enqueue(new Order(Guid.NewGuid()));  // works
        processor.Stop();
        processor.Enqueue(new Order(Guid.NewGuid()));  // not running → throws
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"[InvalidOperationException] {ex.Message}");
    }

    Console.WriteLine();
}

static void SetTimeout(int milliseconds)
{
    if (milliseconds <= 0)
        throw new ArgumentOutOfRangeException(nameof(milliseconds),
            milliseconds, "Timeout must be a positive number of milliseconds.");
    Console.WriteLine($"Timeout set to {milliseconds} ms.");
}

// ──────────────────────────────────────────────
// Supporting types
// ──────────────────────────────────────────────

record Order(Guid Id);

class OrderProcessor
{
    private bool _isRunning = true;
    private readonly List<Order> _queue = [];

    public void Enqueue(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_isRunning)
            throw new InvalidOperationException("Cannot enqueue: the queue is not running.");
        _queue.Add(order);
        Console.WriteLine($"  Enqueued order {order.Id}");
    }

    public void Stop() => _isRunning = false;
}
