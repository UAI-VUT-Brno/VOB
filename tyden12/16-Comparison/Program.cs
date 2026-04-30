// Přehled: kdy použít jakou synchronizační strategii
// dotnet run --project 16-Comparison

// ┌───────────────────────────────────────────────────────────────────────────┐
// │  Situace                               │  Nástroj                        │
// ├───────────────────────────────────────────────────────────────────────────┤
// │  Jednoduchý int/long čítač, příznak    │  Interlocked                    │
// │  Podmíněná atomická aktualizace        │  Interlocked.CompareExchange     │
// │  Stop-flag (volatile bool)             │  volatile / Volatile.Write       │
// │  Mikro-kritická sekce (< 20 ins.)      │  SpinLock                        │
// │  Obecná kritická sekce                 │  lock / Lock.EnterScope() (.NET9)│
// │  Souběžný slovník                      │  ConcurrentDictionary            │
// │  Souběžná fronta (sync)                │  ConcurrentQueue                 │
// │  Ohraničená fronta (back-pressure)     │  BlockingCollection / Channel    │
// │  Async pipeline                        │  Channel<T>                      │
// │  Read-heavy workload                   │  ReaderWriterLockSlim            │
// │  Drahá jednorázová inicializace        │  Lazy<T>                         │
// │  Per-vlákno stav (Random, buffer)      │  ThreadLocal<T>                  │
// │  Per-request stav v async              │  AsyncLocal<T>                   │
// │  Read-only statická data               │  FrozenDictionary / ImmutableXxx │
// │  Sdílený immutabilní objekt (update)   │  Volatile.Write + CAS loop       │
// │  Eliminovat sdílený stav zcela         │  record + immutabilní design     │
// └───────────────────────────────────────────────────────────────────────────┘
//
// Zlaté pravidlo: nejrychlejší synchronizace je ta, která není potřeba.
//   Preferuj immutabilitu a lokální stav před sdíleným mutable stavem.
//   Pokud sdílení nutné: Interlocked > SpinLock > lock > RWLS

// ── Stejný scénář, tři přístupy – čítač návštěv ──────────────────────────
const int N = 8, perThread = 10_000, expected = N * perThread;

int withLock = 0;
var gate = new Lock();
await Run(() => { for (int i = 0; i < perThread; i++) lock (gate) { withLock++; } });
Console.WriteLine($"lock:          {withLock,6} == {expected}: {withLock == expected}");

int withInterlocked = 0;
await Run(() => { for (int i = 0; i < perThread; i++) Interlocked.Increment(ref withInterlocked); });
Console.WriteLine($"Interlocked:   {withInterlocked,6} == {expected}: {withInterlocked == expected}");

VisitStats stats = VisitStats.Zero;
await Run(() =>
{
    for (int i = 0; i < perThread; i++)
    {
        VisitStats snap, next;
        do
        {
            snap = Volatile.Read(ref stats);
            next = snap with { Count = snap.Count + 1 };
        }
        while (Interlocked.CompareExchange(ref stats, next, snap) != snap);
    }
});
Console.WriteLine($"immutable+CAS: {stats.Count,6} == {expected}: {stats.Count == expected}");

static Task Run(Action work) =>
    Task.WhenAll(Enumerable.Range(0, N).Select(_ => Task.Run(work)));

// ── Supporting types ──

record VisitStats(long Count)
{
    public static readonly VisitStats Zero = new(0);
}
