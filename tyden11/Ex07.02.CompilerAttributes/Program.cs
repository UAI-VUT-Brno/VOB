using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

Console.WriteLine("=== 07.02 – Compiler attributes ===\n");

Demo_CompilerAttributes();

// ──────────────────────────────────────────────
// 2. Compiler attributes
// ──────────────────────────────────────────────
static void Demo_CompilerAttributes()
{
    Console.WriteLine("--- Compiler attributes ---");
    // • [DoesNotReturn]: tells the compiler this method always throws (never returns normally).
    //   Enables correct nullability flow analysis and reachability analysis downstream.
    // • [DoesNotReturnIf]: for guard methods that throw conditionally on a bool parameter.
    // • [StackTraceHidden]: hides a method from the stack trace display so developers see
    //   the meaningful call site, not the helper plumbing.
    // • [CallerArgumentExpression]: captures the expression text passed as an argument —
    //   same pattern used by ArgumentNullException.ThrowIfNull in .NET 6+.

    // [DoesNotReturn] — compiler knows Fail() never returns
    try
    {
        string? name = GetName(null);
        Console.WriteLine(name);   // unreachable when user is null — no nullability warning
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"[DoesNotReturn] {ex.Message}");
    }

    string? nameOk = GetName(new User("Alice"));
    Console.WriteLine($"[DoesNotReturn] GetName succeeded: {nameOk}");

    // [CallerArgumentExpression] — captures the expression text
    try
    {
        Order? o = null;
        ThrowIfEmpty(o?.ToString());
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"[CallerArgumentExpression] {ex.Message}");
    }

    Console.WriteLine();
}

static string GetName(User? user)
{
    if (user is null)
        Guard.Fail("User must not be null.");

    return user.Name;   // compiler knows user is non-null here — no warning
}

static void ThrowIfEmpty(
    string? value,
    [CallerArgumentExpression(nameof(value))] string? expr = null)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException($"'{expr}' must not be null or whitespace.", expr);
}

// ── Supporting types ──

record Order(Guid Id);
record User(string Name);

// ── Guard with [DoesNotReturn] + [StackTraceHidden] ──

internal static class Guard
{
    [DoesNotReturn]
    [StackTraceHidden]
    public static void Fail(string message)
        => throw new InvalidOperationException(message);
}
