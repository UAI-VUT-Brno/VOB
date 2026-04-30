// lock + System.Threading.Lock (C# 13 / .NET 9)
// dotnet run --project 02-Lock

// ── Tradiční lock(object) ──────────────────────────────────────────────────
// lock(obj) { } kompiluje se na Monitor.Enter/Exit obalené v try/finally.
// Synchronizační objekt:
//   ✓  private readonly object _gate = new();
//   ❌  lock(this)         – vnější kód může zamknout stejný objekt
//   ❌  lock(typeof(Foo))  – sdílený přes celou AppDomain
//   ❌  lock("string")     – string interning sdílí objekt nečekaně

object gate = new();
int counter = 0;

var tasks = Enumerable.Range(0, 8)
    .Select(_ => Task.Run(() =>
    {
        for (int i = 0; i < 10_000; i++)
            lock (gate) { counter++; }    // ✓ exkluzivní přístup
    }))
    .ToArray();

await Task.WhenAll(tasks);
Console.WriteLine($"lock(object): {counter}");    // vždy 80 000

// ── System.Threading.Lock (.NET 9 / C# 13) ────────────────────────────────
// Nový dedikovaný typ Lock místo obecného object.
//
// Výhody oproti lock(object):
//   • typová bezpečnost – nelze omylem zamknout libovolný objekt
//   • lepší diagnostika v debuggeru a thread-safety analyzátorech
//   • EnterScope() vrátí ref struct Scope → using bez try/finally
//   • JIT rozpozná typ Lock a generuje efektivnější kód

var modernLock = new Lock();
int counter2 = 0;

var tasks2 = Enumerable.Range(0, 8)
    .Select(_ => Task.Run(() =>
    {
        for (int i = 0; i < 10_000; i++)
        {
            using (modernLock.EnterScope())   // ✓ RAII – automaticky uvolní
                counter2++;
        }
    }))
    .ToArray();

await Task.WhenAll(tasks2);
Console.WriteLine($"Lock.EnterScope(): {counter2}");    // vždy 80 000

// lock(modernLock) { ... } funguje také – compiler ho optimalizuje pro typ Lock
// Monitor.TryEnter(gate, timeout) pro případ kdy nechceme čekat věčně (viz 03-Deadlock)
