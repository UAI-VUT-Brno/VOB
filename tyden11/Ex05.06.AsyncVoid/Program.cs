Console.WriteLine("=== 05.06 – async void (event handler pattern) ===\n");

await Demo_AsyncVoid();

// ──────────────────────────────────────────────
// 6. async void — dangerous special case
// ──────────────────────────────────────────────
static async Task Demo_AsyncVoid()
{
    Console.WriteLine("--- async void (event handler pattern) ---");
    // • async void methods have no Task to attach exceptions to — unhandled exceptions
    //   propagate to the sync context and can crash the process.
    // • The only legitimate use case is event handlers, where the delegate signature
    //   mandates void as the return type.
    // • Always wrap the entire body of an async void event handler in try/catch.

    // ✓ GOOD — async Task is awaitable and its exceptions can be caught
    try
    {
        await LoadDataAsync();
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"[async Task] Caught: {ex.Message}");
    }

    // Simulate event handler (async void — must catch internally)
    SimulateButtonClick();

    await Task.Delay(50);  // let the async void complete
    Console.WriteLine();
}

static async Task LoadDataAsync()
{
    await Task.Delay(1);
    throw new InvalidOperationException("DB load failed.");
}

// ✓ Legitimate use of async void — event handler
static async void SimulateButtonClick()
{
    try
    {
        await Task.Delay(1);
        throw new InvalidOperationException("Form submit failed.");
    }
    catch (Exception ex)
    {
        // MUST catch internally — no caller can catch this
        Console.WriteLine($"[async void handler] Handled internally: {ex.Message}");
    }
}
