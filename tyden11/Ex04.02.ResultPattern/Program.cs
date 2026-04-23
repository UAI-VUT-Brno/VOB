Console.WriteLine("=== 04.02 – Result<T> pattern ===\n");

Demo_ResultPattern();

// ──────────────────────────────────────────────
// 2. Result pattern — explicit failure in signature
// ──────────────────────────────────────────────
static void Demo_ResultPattern()
{
    Console.WriteLine("--- Result<T> pattern ---");
    // • Instead of throwing for expected failures, return a discriminated result type that
    //   forces the caller to handle both outcomes.
    // • Result types make failure explicit in the method signature, improving discoverability
    //   and preventing silent error swallowing.
    // • Popular libraries (OneOf, FluentResults, LanguageExt) provide production-ready
    //   implementations; this demo shows the hand-rolled version.

    Guid existing = Guid.NewGuid();
    Guid missing  = Guid.NewGuid();

    var db = new FakeOrderDb(existing);

    PrintResult(db.GetOrder(existing));
    PrintResult(db.GetOrder(missing));

    Console.WriteLine();
}

static void PrintResult(Result<Order> result)
{
    if (result.IsSuccess)
        Console.WriteLine($"  Found order: {result.Value!.Id}");
    else
        Console.WriteLine($"  Not found: {result.Error}");
}

// ── Supporting types ──

record Order(Guid Id);

// ── Simple Result<T> ──

readonly struct Result<T>
{
    public T? Value    { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }

    private Result(T value) { Value = value; IsSuccess = true; }
    private Result(string error) { Error = error; IsSuccess = false; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
}

class FakeOrderDb(Guid existingId)
{
    public Result<Order> GetOrder(Guid id)
    {
        if (id == existingId)
            return Result<Order>.Success(new Order(id));
        return Result<Order>.Failure($"Order {id} not found.");
    }
}
