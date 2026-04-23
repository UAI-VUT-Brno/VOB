Console.WriteLine("=== 01.05 – Exception propagation ===\n");

Demo_ExceptionPropagation();

// ──────────────────────────────────────────────
// 5. Exception propagation chain
// ──────────────────────────────────────────────
static void Demo_ExceptionPropagation()
{
    Console.WriteLine("--- Exception propagation ---");
    // • Unhandled exceptions travel up the call stack frame by frame until a matching
    //   catch is found or the process terminates.
    // • Each InnerException in a chain points to the original cause, forming a traceable exception tree.
    // • The stack trace is captured at the first throw site; bare re-throws preserve it
    //   while throw ex resets it.

    try
    {
        OrderService_PlaceOrder();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Caught at top level: {ex.GetType().Name} — {ex.Message}");
        if (ex.InnerException is not null)
            Console.WriteLine($"  InnerException: {ex.InnerException.GetType().Name} — {ex.InnerException.Message}");
    }

    Console.WriteLine();
}

static void OrderService_PlaceOrder() => PaymentGateway_Charge();

static void PaymentGateway_Charge()
{
    try
    {
        HttpClient_SendAsync(); // simulates IOException
    }
    catch (System.IO.IOException ex)
    {
        throw new InvalidOperationException("Payment gateway unreachable.", ex);
    }
}

static void HttpClient_SendAsync() =>
    throw new System.IO.IOException("Connection refused.");
