Console.WriteLine("=== 07.03 – Resilience: retry with exponential back-off ===\n");

await Demo_ResilienceRetry();

// ──────────────────────────────────────────────
// 3. Resilience — simple retry (Polly-style logic, no NuGet dependency)
// ──────────────────────────────────────────────
static async Task Demo_ResilienceRetry()
{
    Console.WriteLine("--- Resilience: retry with exponential back-off ---");
    // • Polly is the de facto standard for transient-fault handling in .NET — retry,
    //   circuit breaker, timeout, and bulkhead isolation.
    // • Policies are composable and ordered: outer policies (total timeout) wrap inner ones
    //   (retry wraps circuit breaker).
    // • Polly v8 introduces a unified ResiliencePipeline API and integrates directly with
    //   IHttpClientFactory via Microsoft.Extensions.Http.Resilience.
    // • This demo implements the same retry and circuit-breaker concepts without the NuGet
    //   dependency so the project stays self-contained.

    int attempt = 0;

    try
    {
        await RetryAsync(
            operation: async ct =>
            {
                attempt++;
                Console.WriteLine($"  Attempt {attempt}...");
                await Task.Delay(5, ct);

                if (attempt < 3)
                    throw new InvalidOperationException("Transient failure.");

                Console.WriteLine($"  Attempt {attempt} succeeded.");
            },
            maxAttempts: 5,
            baseDelay: TimeSpan.FromMilliseconds(10));
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  All retries exhausted: {ex.Message}");
    }

    Console.WriteLine();

    // Circuit breaker concept — show open-circuit behaviour
    Console.WriteLine("--- Circuit breaker concept ---");
    var breaker = new SimpleCircuitBreaker(failureThreshold: 2, breakDuration: TimeSpan.FromMilliseconds(80));

    for (int i = 0; i < 5; i++)
    {
        try
        {
            await breaker.ExecuteAsync(_ =>
            {
                if (i < 3) throw new InvalidOperationException($"Call {i + 1} failed.");
                Console.WriteLine($"  Call {i + 1} succeeded (circuit closed again).");
                return Task.CompletedTask;
            }, CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Call {i + 1}: {ex.Message}");
        }
        catch (CircuitOpenException)
        {
            Console.WriteLine($"  Call {i + 1}: Circuit is OPEN — request rejected immediately.");
        }

        await Task.Delay(50);  // simulate time passing (triggers half-open after break duration)
    }

    Console.WriteLine();
}

static async Task RetryAsync(
    Func<CancellationToken, Task> operation,
    int maxAttempts,
    TimeSpan baseDelay,
    CancellationToken ct = default)
{
    for (int i = 1; i <= maxAttempts; i++)
    {
        try
        {
            await operation(ct);
            return;
        }
        catch (Exception) when (i < maxAttempts)
        {
            var delay = baseDelay * Math.Pow(2, i - 1);   // exponential back-off
            Console.WriteLine($"    Retry in {delay.TotalMilliseconds:F0} ms...");
            await Task.Delay(delay, ct);
        }
    }
}

// ── Supporting types ──

public sealed class CircuitOpenException() : Exception("Circuit breaker is open.");

enum CircuitState { Closed, Open, HalfOpen }

sealed class SimpleCircuitBreaker(int failureThreshold, TimeSpan breakDuration)
{
    private CircuitState _state = CircuitState.Closed;
    private int _failures;
    private DateTimeOffset _openedAt;

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTimeOffset.UtcNow - _openedAt >= breakDuration)
                _state = CircuitState.HalfOpen;
            else
                throw new CircuitOpenException();
        }

        try
        {
            await action(ct);
            _failures = 0;
            _state = CircuitState.Closed;
        }
        catch when (_state != CircuitState.Open)
        {
            _failures++;
            if (_failures >= failureThreshold)
            {
                _state = CircuitState.Open;
                _openedAt = DateTimeOffset.UtcNow;
                Console.WriteLine($"  [circuit] Circuit OPENED after {_failures} failures.");
            }
            throw;
        }
    }
}
