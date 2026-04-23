Console.WriteLine("=== 03.04 – Exception filters (when) ===\n");

Demo_ExceptionFilters();

// ──────────────────────────────────────────────
// 4. Exception filters (when clause)
// ──────────────────────────────────────────────
static void Demo_ExceptionFilters()
{
    Console.WriteLine("--- Exception filters (when) ---");
    // • The when clause evaluates a condition before deciding whether to enter a catch block —
    //   the stack is NOT unwound if the filter returns false.
    // • This is fundamentally different from catching and re-throwing: no frames are released
    //   unless the filter returns true.
    // • Filters enable conditional handling based on exception properties (e.g., HTTP status code)
    //   without nesting if inside catch.
    // • The side-effect trick (when (Log(ex)) returning false) allows pure observation
    //   without altering propagation.

    // Filter by error code
    try
    {
        ThrowHttpError(503);
    }
    catch (AppHttpException ex) when (ex.StatusCode == 503)
    {
        Console.WriteLine($"[filter 503] Service unavailable — handled: {ex.Message}");
    }

    // Filter: catch only non-cancellation exceptions
    try
    {
        throw new OperationCanceledException("User cancelled.");
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        Console.WriteLine("Should not reach here.");
    }
    catch (OperationCanceledException ex)
    {
        Console.WriteLine($"[filter not-cancelled] Cancellation propagated: {ex.Message}");
    }

    // Log-and-rethrow using filter side effect
    try
    {
        try
        {
            throw new InvalidOperationException("Some failure.");
        }
        catch (Exception ex) when (LogAndReturnFalse(ex))
        {
            // never reached — filter returned false
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[filter side-effect] Exception propagated: {ex.Message}");
    }

    Console.WriteLine();
}

static bool LogAndReturnFalse(Exception ex)
{
    Console.WriteLine($"  [observer] Logged: {ex.Message}");
    return false;   // do not catch — just observe
}

static void ThrowHttpError(int code) =>
    throw new AppHttpException(code, $"HTTP {code} response received.");

// ── Supporting types ──

public sealed class AppHttpException(int statusCode, string message)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
