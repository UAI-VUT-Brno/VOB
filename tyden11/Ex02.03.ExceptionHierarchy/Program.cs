Console.WriteLine("=== 02.03 – Exception hierarchy ===\n");

Demo_ExceptionHierarchy();

// ──────────────────────────────────────────────
// 3. Exception hierarchy — abstract base + sealed specifics
// ──────────────────────────────────────────────
static void Demo_ExceptionHierarchy()
{
    Console.WriteLine("--- Exception hierarchy ---");
    // • A shared abstract base exception lets callers catch all related domain failures
    //   with a single catch block.
    // • Place common structured properties (e.g., OrderId) on the base class to avoid
    //   duplication across subclasses.
    // • Keep the hierarchy shallow — two levels (abstract base + sealed specifics) are
    //   almost always sufficient.

    Guid orderId = Guid.NewGuid();
    Exception[] exceptions =
    [
        new OrderNotFoundException(orderId),
        new OrderAlreadyShippedException(orderId),
    ];

    foreach (var ex in exceptions)
    {
        try { throw ex; }
        catch (OrderException oex)
        {
            // single catch for all order exceptions
            Console.WriteLine($"[OrderException catch] {oex.GetType().Name}: {oex.Message} (OrderId={oex.OrderId})");
        }
    }

    Console.WriteLine();
}

// ── Abstract base for order-domain exceptions ──

public abstract class OrderException : Exception
{
    public Guid OrderId { get; }

    protected OrderException(Guid orderId, string message)
        : base(message) => OrderId = orderId;

    protected OrderException(Guid orderId, string message, Exception inner)
        : base(message, inner) => OrderId = orderId;
}

public sealed class OrderNotFoundException : OrderException
{
    public OrderNotFoundException(Guid orderId)
        : base(orderId, $"Order '{orderId}' was not found.") { }
}

public sealed class OrderAlreadyShippedException : OrderException
{
    public OrderAlreadyShippedException(Guid orderId)
        : base(orderId, $"Order '{orderId}' has already been shipped.") { }
}
