Console.WriteLine("=== 06.03 – UnobservedTaskException safety net ===\n");

// ── Global handlers setup ──
// • The last safety net for unhandled exceptions in non-web processes — fires when no
//   catch block handles the exception on any thread.
// • On .NET Core / .NET 5+, IsTerminating is always true — the process will crash;
//   use this handler only to log and flush.
// • Do not attempt recovery here; the process state is undefined at this point.
// • This handler is intentionally NOT triggered in this demo — doing so would crash the process.
AppDomain.CurrentDomain.UnhandledException += (_, args) =>
{
    var ex = (Exception)args.ExceptionObject;
    Console.WriteLine($"[AppDomain.UnhandledException] {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine($"  IsTerminating: {args.IsTerminating}");
    // In real code: flush log buffers here before process exits
};

// ── Unobserved task exception handler ──
// • Fires when a faulted Task is finalized without its exception ever being observed.
// • Acts as a safety net to surface otherwise-silent async failures; the real fix is
//   always to await every Task.
TaskScheduler.UnobservedTaskException += (_, args) =>
{
    Console.WriteLine($"[TaskScheduler.UnobservedTaskException] {args.Exception.InnerException?.Message}");
    args.SetObserved();  // prevent finalizer from re-raising
};

await Demo_UnobservedTask();

Console.WriteLine("=== Done ===");

// ──────────────────────────────────────────────
// 3. Unobserved task exception (safety-net demo)
// ──────────────────────────────────────────────
static async Task Demo_UnobservedTask()
{
    Console.WriteLine("--- UnobservedTaskException safety net ---");

    // Fire-and-forget faulted task
    _ = Task.Run(() => throw new InvalidOperationException("Abandoned task failure."));

    await Task.Delay(50);

    // Force GC to finalize the abandoned Task
    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine("  (the UnobservedTaskException event fires on finalization)");
    Console.WriteLine("  → Real fix: always await every Task.");
    Console.WriteLine();
}
