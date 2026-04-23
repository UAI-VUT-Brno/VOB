using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

Console.WriteLine("=== 07.01 – ThrowHelper pattern ===\n");

Demo_ThrowHelperPattern();

// ──────────────────────────────────────────────
// 1. ThrowHelper pattern
// ──────────────────────────────────────────────
static void Demo_ThrowHelperPattern()
{
    Console.WriteLine("--- ThrowHelper pattern ---");
    // • The JIT cannot inline a method that contains a throw statement because the exception
    //   setup code is too large.
    // • Moving the throw into a separate [MethodImpl(NoInlining)] helper keeps the fast path
    //   eligible for inlining.
    // • This pattern is used extensively inside the BCL (System.Collections, System.Text.Json,
    //   System.Runtime).
    // • Returning T from the helper allows use in expression-bodied contexts (e.g., ternary).

    // ✓ Inlining-friendly validation
    try
    {
        Validate(null!);
    }
    catch (ArgumentNullException ex)
    {
        Console.WriteLine($"[ThrowHelper] {ex.Message}");
    }

    // GetOrThrow throws when the key is not found — demo it safely:
    try
    {
        _ = GetOrThrow(new Dictionary<Guid, Order>(), Guid.NewGuid());
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"[ThrowHelper expression] {ex.Message}");
    }

    Console.WriteLine();
}

static void Validate(string input)
{
    if (input is null)
        ThrowHelper.ThrowArgumentNull(nameof(input));   // JIT can inline Validate()
}

static Order GetOrThrow(Dictionary<Guid, Order> cache, Guid id)
    => cache.TryGetValue(id, out var order)
        ? order
        : ThrowHelper.ThrowKeyNotFound<Order>(id.ToString());

// ── Supporting types ──

record Order(Guid Id);

// ── ThrowHelper ──

internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNull(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperation(string message)
        => throw new InvalidOperationException(message);

    // Returning T allows use in expression contexts
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentNull<T>(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowKeyNotFound(string key)
        => throw new KeyNotFoundException($"Key '{key}' was not found.");

    // Returning T allows use in expression contexts (e.g., ternary)
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowKeyNotFound<T>(string key)
        => throw new KeyNotFoundException($"Key '{key}' was not found.");
}
