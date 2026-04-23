Console.WriteLine("=== 01.02 – throw vs throw ex ===\n");

Demo_ThrowVsThrowEx();

// ──────────────────────────────────────────────
// 2. throw vs throw ex — stack trace preservation
// ──────────────────────────────────────────────
static void Demo_ThrowVsThrowEx()
{
    Console.WriteLine("--- throw vs throw ex ---");
    // • throw ex resets the stack trace to the current line — the original throw site disappears
    //   from logs and debuggers. This is one of the most common mistakes in C#.
    // • Bare throw (no argument) re-throws the active exception while fully preserving
    //   the original stack trace.
    // • ExceptionDispatchInfo.Capture(ex).Throw() enables re-throwing across thread or
    //   async boundaries with the original trace intact.

    // BAD: throw ex resets stack trace
    try
    {
        try
        {
            ThrowOriginal();
        }
        catch (Exception ex)
        {
            // ⚠ BAD – resets the stack trace
#pragma warning disable CA2200  // intentional: demonstrating the bad pattern
            throw ex;   // stack trace now starts HERE
#pragma warning restore CA2200
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[throw ex]  First frame: {ex.StackTrace?.Split('\n')[0].Trim()}");
    }

    // GOOD: bare throw preserves stack trace
    try
    {
        try
        {
            ThrowOriginal();
        }
        catch (Exception)
        {
            // ✓ GOOD – preserves the original stack trace
            throw;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[throw]     First frame: {ex.StackTrace?.Split('\n')[0].Trim()}");
    }

    Console.WriteLine();
}

static void ThrowOriginal() => throw new InvalidOperationException("Original exception.");
