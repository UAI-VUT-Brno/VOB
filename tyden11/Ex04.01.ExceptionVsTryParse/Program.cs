using System.Diagnostics;

Console.WriteLine("=== 04.01 – Exception cost: throw vs TryParse ===\n");

Demo_ExceptionVsTryParse();

// ──────────────────────────────────────────────
// 1. Exception on expected failure vs. TryParse
// ──────────────────────────────────────────────
static void Demo_ExceptionVsTryParse()
{
    Console.WriteLine("--- Exception cost: throw vs TryParse ---");
    // • Every throw involves stack trace capture, handler search, stack unwinding, and heap
    //   allocation — several orders of magnitude slower than a normal method call.
    // • The cost is negligible for truly rare failures but becomes a bottleneck in loops
    //   that throw on every iteration.
    // • Profiling tools (BenchmarkDotNet, dotnet-trace, PerfView) can confirm whether
    //   exceptions are on your critical path.

    string[] inputs = GenerateInputs(200);
    int iterations = 1000;

    // ⚠ BAD: exceptions for expected failures
    var sw1 = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
        ParseAllWithException(inputs);
    sw1.Stop();

    // ✓ GOOD: TryParse — no allocation on failure
    var sw2 = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
        ParseAllWithTryParse(inputs);
    sw2.Stop();

    Console.WriteLine($"  ParseAll with exceptions : {sw1.ElapsedMilliseconds,6} ms");
    Console.WriteLine($"  ParseAll with TryParse   : {sw2.ElapsedMilliseconds,6} ms");
    Console.WriteLine($"  Ratio: ~{(double)sw1.ElapsedMilliseconds / Math.Max(1, sw2.ElapsedMilliseconds):F1}×");
    Console.WriteLine();
}

static string[] GenerateInputs(int count)
{
    // half valid integers, half invalid
    var list = new string[count];
    for (int i = 0; i < count; i++)
        list[i] = i % 2 == 0 ? i.ToString() : $"x{i}";
    return list;
}

// ⚠ Slow when inputs frequently invalid
static int[] ParseAllWithException(string[] inputs)
{
    var results = new List<int>();
    foreach (var input in inputs)
    {
        try { results.Add(int.Parse(input)); }
        catch (FormatException) { /* skip */ }
    }
    return results.ToArray();
}

// ✓ Correct approach for expected failures
static int[] ParseAllWithTryParse(string[] inputs)
{
    var results = new List<int>(inputs.Length);
    foreach (var input in inputs)
    {
        if (int.TryParse(input, out int value))
            results.Add(value);
    }
    return results.ToArray();
}
