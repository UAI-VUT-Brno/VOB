Console.WriteLine("=== 04.04 – Benchmark summary ===\n");

Demo_BenchmarkSummary();

// ──────────────────────────────────────────────
// 4. Benchmark summary table
// ──────────────────────────────────────────────
static void Demo_BenchmarkSummary()
{
    Console.WriteLine("--- Relative cost summary ---");
    // • These figures are order-of-magnitude approximations; actual cost varies with
    //   stack depth, JIT state, and CPU cache.
    // • Do not optimize preemptively — profile first and replace exceptions with Result
    //   only if they appear on a confirmed hot path.
    // • For operations called < 1 000×/sec, the overhead is irrelevant.
    Console.WriteLine("  Operation                              | Relative cost");
    Console.WriteLine("  ---------------------------------------|---------------");
    Console.WriteLine("  Normal method call                     | 1×");
    Console.WriteLine("  TryParse (failure path)                | ~2×");
    Console.WriteLine("  Exception thrown + caught locally      | ~200–2 000×");
    Console.WriteLine("  Exception with deep stack trace        | ~10 000×");
    Console.WriteLine();
    Console.WriteLine("  Optimize only after profiling confirms exceptions are on a hot path.");
    Console.WriteLine();
}
