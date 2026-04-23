Console.WriteLine("=== 03.05 – Re-throw patterns ===\n");

Demo_RethrowPatterns();

// ──────────────────────────────────────────────
// 5. Re-throw patterns
// ──────────────────────────────────────────────
static void Demo_RethrowPatterns()
{
    Console.WriteLine("--- Re-throw patterns ---");
    // • Bare throw preserves the original stack trace; throw ex resets it — never use the latter.
    // • Wrapping in a new exception with inner preserves the original cause while adding
    //   context appropriate to the current layer.
    // • Choose the pattern based on whether you are adding information (wrap) or purely
    //   propagating (bare throw).

    // ✓ Bare throw — preserves stack trace
    try
    {
        try { throw new InvalidOperationException("original"); }
        catch (Exception ex)
        {
            Console.WriteLine($"  Logging before re-throw: {ex.Message}");
            throw;  // preserves stack trace
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[bare throw] Caught: {ex.Message}");
    }

    // ✓ Wrapping — preserves origin via InnerException
    try
    {
        try { throw new System.IO.IOException("disk full"); }
        catch (System.IO.IOException ex)
        {
            throw new DataAccessException("Could not persist the report.", ex);
        }
    }
    catch (DataAccessException ex)
    {
        Console.WriteLine($"[wrap] {ex.Message} | inner: {ex.InnerException?.Message}");
    }

    Console.WriteLine();
}

// ── Supporting types ──

public sealed class DataAccessException(string message, Exception? inner = null)
    : Exception(message, inner);
