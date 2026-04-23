Console.WriteLine("=== 01.01 – try / catch / finally ===\n");

Demo_TryCatchFinally();

// ──────────────────────────────────────────────
// 1. try / catch / finally
// ──────────────────────────────────────────────
static void Demo_TryCatchFinally()
{
    Console.WriteLine("--- try/catch/finally ---");
    // • Divides code into guarded execution (try), targeted error handling (catch),
    //   and guaranteed cleanup (finally).
    // • Multiple catch blocks are evaluated top-to-bottom; only the first matching block executes.
    // • finally always runs on both success and failure paths — ideal for releasing unmanaged resources.
    // • finally runs even if catch re-throws, and even if there is no catch block at all.
    // • Fatal CLR exceptions (StackOverflowException, ExecutionEngineException) bypass finally entirely.

    string input = "hello";

    try
    {
        int result = Process(input);
        Console.WriteLine($"Result: {result}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"[catch InvalidOperationException] {ex.Message}");
        // re-throw would propagate up; here we just show the message
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"[catch FormatException] {ex.Message}");
    }
    finally
    {
        Console.WriteLine("[finally] Cleanup always runs.");
    }

    Console.WriteLine();
}

static int Process(string input)
{
    if (!int.TryParse(input, out int value))
        throw new FormatException($"Input '{input}' is not a valid integer.");
    return value * 2;
}
