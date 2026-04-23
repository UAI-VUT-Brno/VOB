Console.WriteLine("=== 06.02 – Custom exception middleware (pipeline simulation) ===\n");

Demo_CustomMiddlewarePattern();

Console.WriteLine("=== Done ===");

// ──────────────────────────────────────────────
// 2. Custom middleware pattern (without ASP.NET Core)
// ──────────────────────────────────────────────
static void Demo_CustomMiddlewarePattern()
{
    Console.WriteLine("--- Custom exception middleware (pipeline simulation) ---");
    // • All unhandled exceptions should be centralized in a single exception-handling layer.
    // • Centralization prevents duplicate logging, inconsistent HTTP status codes, and
    //   implementation details leaking to clients.
    // • Use IExceptionHandler (.NET 8+) for a composable, DI-friendly alternative to a
    //   single monolithic middleware class.

    var pipeline = new RequestPipeline();

    string[] routes = ["/orders/valid", "/orders/notfound", "/orders/crash"];

    foreach (var route in routes)
    {
        var response = pipeline.Handle(route);
        Console.WriteLine($"  {route,-25} → {response.StatusCode} {response.Body}");
    }

    Console.WriteLine();
}

// ── Supporting types ──

// Lightweight middleware pipeline simulation
sealed record HttpResponse(int StatusCode, string Body);

class RequestPipeline
{
    public HttpResponse Handle(string route)
    {
        try
        {
            return Dispatch(route);
        }
        catch (NotFoundException ex)
        {
            return new HttpResponse(404, $"Not Found: {ex.Message}");
        }
        catch (Exception ex)
        {
            // log ex in real code
            return new HttpResponse(500, $"Unexpected error: {ex.Message}");
        }
    }

    private static HttpResponse Dispatch(string route) => route switch
    {
        "/orders/valid"    => new HttpResponse(200, "{ \"id\": 1 }"),
        "/orders/notfound" => throw new NotFoundException("Order not found."),
        _                  => throw new InvalidOperationException("Unhandled route.")
    };
}

public sealed class NotFoundException(string message) : Exception(message);
