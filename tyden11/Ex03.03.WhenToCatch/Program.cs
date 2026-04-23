Console.WriteLine("=== 03.03 – When to catch ===\n");

Demo_WhenToCatch();

// ──────────────────────────────────────────────
// 3. When to catch — retry, translate, log
// ──────────────────────────────────────────────
static void Demo_WhenToCatch()
{
    Console.WriteLine("--- When to catch ---");
    // • Only catch an exception when you can genuinely react: recover, provide a fallback,
    //   or translate to a domain exception.
    // • Catching at the wrong layer forces higher-level code to make decisions without
    //   sufficient context.
    // • Letting exceptions propagate to a system boundary (API controller, job runner, UI)
    //   is often the correct strategy.
    // • Never catch to immediately discard (silent swallow) — failures disappear silently.

    // Translate infrastructure exception to domain exception
    Guid id = Guid.NewGuid();
    try
    {
        GetOrder(id);
    }
    catch (DataAccessException ex)
    {
        Console.WriteLine($"[translate] {ex.Message} | inner: {ex.InnerException?.GetType().Name}");
    }

    // Silent swallow — never do this; shown for contrast only
    Console.WriteLine("[swallow ⚠] (not demonstrated — it silently hides errors)");

    Console.WriteLine();
}

static Order GetOrder(Guid id)
{
    try
    {
        // Simulate a SqlException-like infrastructure failure
        throw new InvalidOperationException("Simulated DB error.");
    }
    catch (InvalidOperationException ex)
    {
        throw new DataAccessException("Database unavailable.", ex);
    }
}

// ── Supporting types ──

record Order(Guid Id, decimal Total);

public sealed class DataAccessException(string message, Exception? inner = null)
    : Exception(message, inner);
