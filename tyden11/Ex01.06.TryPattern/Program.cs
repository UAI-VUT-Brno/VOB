Console.WriteLine("=== 01.06 – TryXxx pattern ===\n");

Demo_TryPattern();

// ──────────────────────────────────────────────
// 6. TryXxx pattern
// ──────────────────────────────────────────────
static void Demo_TryPattern()
{
    Console.WriteLine("--- TryXxx pattern ---");
    // • The TryXxx pattern returns bool and uses an out parameter to avoid exception
    //   allocation on expected failures.
    // • Use TryXxx variants in hot paths or when failure is a normal, frequent outcome
    //   (e.g., parsing user input).
    // • Use throwing variants when failure is truly unexpected and callers must be forced to handle it.

    string[] inputs = { "42", "abc", "100" };

    foreach (var input in inputs)
    {
        // TryParse — no exception on invalid input
        if (int.TryParse(input, out int value))
            Console.WriteLine($"[TryParse] '{input}' parsed as {value}");
        else
            Console.WriteLine($"[TryParse] '{input}' is not a valid integer.");
    }

    // Throwing variant — only safe when input is trusted
    try
    {
        int v = int.Parse("not-a-number");
        Console.WriteLine(v);
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"[int.Parse] threw: {ex.Message}");
    }

    Console.WriteLine();
}
