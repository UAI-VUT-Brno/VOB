Console.WriteLine("=== 04.03 – BCL TryXxx methods ===\n");

Demo_TryXxxBclMethods();

// ──────────────────────────────────────────────
// 3. BCL TryXxx methods
// ──────────────────────────────────────────────
static void Demo_TryXxxBclMethods()
{
    Console.WriteLine("--- BCL TryXxx methods ---");
    // • The BCL consistently uses the TryXxx naming convention for non-throwing parsing
    //   and lookup operations.
    // • Apply the same convention in your own types to provide a familiar, predictable API surface.
    // • Return bool + out parameter; never throw on the failure path.

    Console.WriteLine($"  int.TryParse(\"42\")          : {int.TryParse("42", out int n1)} → {n1}");
    Console.WriteLine($"  int.TryParse(\"nope\")        : {int.TryParse("nope", out int n2)} → {n2}");
    Console.WriteLine($"  Guid.TryParse(valid)        : {Guid.TryParse(Guid.NewGuid().ToString(), out Guid g)}");

    var dict = new Dictionary<string, int> { ["a"] = 1 };
    Console.WriteLine($"  dict.TryGetValue(\"a\")       : {dict.TryGetValue("a", out int v)} → {v}");
    Console.WriteLine($"  dict.TryGetValue(\"z\")       : {dict.TryGetValue("z", out int v2)} → {v2}");

    // Custom TryXxx
    var cache = new OrderCache();
    cache.Add(new Order(Guid.NewGuid()));
    var id = cache.All()[0].Id;

    if (cache.TryFindOrder(id, out var found))
        Console.WriteLine($"  TryFindOrder(existing)     : found {found!.Id}");

    Console.WriteLine($"  TryFindOrder(Guid.Empty)   : {cache.TryFindOrder(Guid.Empty, out _)}");

    Console.WriteLine();
}

// ── Supporting types ──

record Order(Guid Id);

class OrderCache
{
    private readonly List<Order> _items = [];

    public void Add(Order o) => _items.Add(o);
    public IReadOnlyList<Order> All() => _items;

    public bool TryFindOrder(Guid id, out Order? order)
    {
        order = _items.FirstOrDefault(o => o.Id == id);
        return order is not null;
    }
}
