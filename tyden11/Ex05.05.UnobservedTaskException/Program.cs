Console.WriteLine("=== 05.05 – UnobservedTaskException ===\n");

Demo_UnobservedTaskException();

// ──────────────────────────────────────────────
// 5. UnobservedTaskException safety net
// ──────────────────────────────────────────────
static void Demo_UnobservedTaskException()
{
    Console.WriteLine("--- UnobservedTaskException ---");
    // • Fires when a faulted Task is garbage collected without its exception being observed
    //   by any code.
    // • In .NET 4.5+ this no longer crashes the process by default, but failures disappear
    //   silently — subscribe as a safety net.
    // • The real fix is always to await every Task; the subscription is a last-resort guard.

    TaskScheduler.UnobservedTaskException += (_, args) =>
    {
        Console.WriteLine($"  [UnobservedTaskException] {args.Exception.InnerException?.Message}");
        args.SetObserved();  // prevent process crash
    };

    // Fire-and-forget task that faults — nobody awaits it
    _ = Task.Run(() => throw new InvalidOperationException("Forgotten task failed."));

    // Force GC to finalize the abandoned Task so the event fires
    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine("  (event fires when abandoned faulted Task is GC-finalized)");
    Console.WriteLine("  → Real fix: always await every Task.");
    Console.WriteLine();
}
