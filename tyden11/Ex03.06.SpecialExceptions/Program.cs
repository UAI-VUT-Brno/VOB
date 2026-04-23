Console.WriteLine("=== 03.06 – Special exception types ===\n");

Demo_SpecialExceptions();

// ──────────────────────────────────────────────
// 6. Special exception types to be careful with
// ──────────────────────────────────────────────
static void Demo_SpecialExceptions()
{
    Console.WriteLine("--- Special exception types ---");
    // • Several BCL exception types carry special semantics and must either never be caught
    //   or always be re-thrown after cleanup.
    // • OperationCanceledException / TaskCanceledException: always re-throw after cleanup —
    //   suppressing it breaks the cooperative cancellation chain across the entire call tree.
    // • NullReferenceException / IndexOutOfRangeException: indicate bugs — fix the code, never catch.
    // • StackOverflowException: cannot be caught; terminates the process.
    //   ExecutionEngineException was similar in .NET Framework but is obsolete in .NET Core+ and never thrown by the runtime.

    // OperationCanceledException — re-throw after cleanup
    using var cts = new CancellationTokenSource();
    cts.Cancel();

    try
    {
        cts.Token.ThrowIfCancellationRequested();
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("[OperationCanceledException] Cleaned up, re-throwing.");
        // In real code: throw;  — don't suppress cancellation
        Console.WriteLine("  (re-throw omitted here to keep demo running)");
    }

    // NullReferenceException — fix the bug, never catch
    Console.WriteLine("[NullReferenceException] Fix the null dereference bug — don't catch it.");

    Console.WriteLine();
}
