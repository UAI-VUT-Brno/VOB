Console.WriteLine("=== 03.02 – Don't throw for control flow ===\n");

Demo_WhenNotToThrow();

// ──────────────────────────────────────────────
// 2. Do NOT throw for expected control flow
// ──────────────────────────────────────────────
static void Demo_WhenNotToThrow()
{
    Console.WriteLine("--- Don't throw for control flow ---");
    // • Never use exceptions as if/else — it is a performance and readability problem.
    // • Use TryXxx methods when failure is an expected, frequent outcome (e.g., user input).
    // • The throwing variant (int.Parse) is appropriate only when failure is truly unexpected.

    string[] inputs = ["25", "abc", "200", "-1"];
    foreach (var s in inputs)
    {
        Console.WriteLine($"  '{s}' → isValidAge={IsValidAge(s)}");
    }

    Console.WriteLine();
}

// ✓ GOOD — no exception for invalid input
static bool IsValidAge(string input)
    => int.TryParse(input, out int age) && age is >= 0 and <= 150;
