Console.WriteLine("=== 02.02 – Custom exception ===\n");

Demo_CustomException();

// ──────────────────────────────────────────────
// 2. Custom exception — minimal correct form
// ──────────────────────────────────────────────
static void Demo_CustomException()
{
    Console.WriteLine("--- Custom exception (OrderNotFoundException) ---");
    // • Create a custom exception when a built-in type cannot convey enough domain context
    //   (e.g., which entity ID was missing).
    // • Always provide both a plain constructor and one accepting Exception inner —
    //   wrapping context is needed at boundaries.
    // • Expose structured data as typed properties, not only in the message string,
    //   so callers can react programmatically.
    // • On .NET 5+, binary serialization constructors (SerializationInfo) are obsolete
    //   and should be omitted in new code.

    Guid missingId = Guid.NewGuid();

    try
    {
        throw new OrderNotFoundException(missingId);
    }
    catch (OrderNotFoundException ex)
    {
        Console.WriteLine($"Caught: {ex.Message}");
        Console.WriteLine($"OrderId property: {ex.OrderId}");
    }

    // Wrapped in inner exception
    try
    {
        try
        {
            throw new System.IO.IOException("DB read failed.");
        }
        catch (System.IO.IOException ex)
        {
            throw new OrderNotFoundException(missingId, ex);
        }
    }
    catch (OrderNotFoundException ex)
    {
        Console.WriteLine($"Wrapped: {ex.Message} | InnerException: {ex.InnerException?.Message}");
    }

    Console.WriteLine();
}

// ── Custom exception ──

public sealed class OrderNotFoundException : Exception
{
    public Guid OrderId { get; }

    public OrderNotFoundException(Guid orderId)
        : base($"Order '{orderId}' was not found.")
    {
        OrderId = orderId;
    }

    public OrderNotFoundException(Guid orderId, Exception inner)
        : base($"Order '{orderId}' was not found.", inner)
    {
        OrderId = orderId;
    }
}
