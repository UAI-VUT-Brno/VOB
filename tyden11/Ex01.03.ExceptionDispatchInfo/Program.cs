using System.Runtime.ExceptionServices;

Console.WriteLine("=== 01.03 – ExceptionDispatchInfo ===\n");

Demo_ExceptionDispatchInfo();

// ──────────────────────────────────────────────
// 3. ExceptionDispatchInfo — re-throw across async/thread boundaries
// ──────────────────────────────────────────────
static void Demo_ExceptionDispatchInfo()
{
    Console.WriteLine("--- ExceptionDispatchInfo ---");
    // • Use ExceptionDispatchInfo when you need to re-throw an exception captured across
    //   an async boundary or thread while preserving its original stack trace.
    // • Capture() stores the exception together with its current stack state.
    // • Throw() re-raises it so the stack trace shows both the original site and the re-throw point.

    ExceptionDispatchInfo? captured = null;

    try
    {
        ThrowOriginal();
    }
    catch (Exception ex)
    {
        captured = ExceptionDispatchInfo.Capture(ex);
        Console.WriteLine($"Captured: {ex.Message}");
    }

    // Re-throw later (possibly on a different thread) with the original stack trace intact
    try
    {
        captured?.Throw();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Re-thrown with EDI — original site visible in stack trace: {ex.Message}");
    }

    Console.WriteLine();
}

static void ThrowOriginal() => throw new InvalidOperationException("Original exception.");
