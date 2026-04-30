// ConcurrentQueue<T> – lock-free thread-safe fronta
// dotnet run --project 08-ConcurrentQueue

// Queue<T> NENÍ thread-safe – Enqueue/Dequeue z více vláken poškodí strukturu.
//
// ConcurrentQueue<T> implementuje FIFO pomocí CPU-level CAS instrukcí (lock-free):
//   • Enqueue      – vždy úspěšné, žádná omezená kapacita
//   • TryDequeue   – vrátí false místo výjimky při prázdné frontě
//   • TryPeek      – přečte první prvek bez odebrání, atomic snapshot
//   • IsEmpty      – atomický dotaz, bezpečný souběžně s Enqueue
//   • Count        – přibližný (může být zastaralý v okamžiku čtení – to je OK pro monitoring)
//
// Kdy použít:
//   ✓  work queue, event bus, log pipeline – producenti a konzumenti bez koordinace
//   ❌  potřebuji back-pressure (omezit producenta) → BlockingCollection nebo Channel

var queue = new System.Collections.Concurrent.ConcurrentQueue<string>();
int produced = 0, consumed = 0;

// Tři producenti enqueueují souběžně
var producers = Enumerable.Range(1, 3).Select(p => Task.Run(async () =>
{
    for (int i = 1; i <= 4; i++)
    {
        queue.Enqueue($"P{p}-msg{i}");
        Interlocked.Increment(ref produced);
        await Task.Delay(10);
    }
})).ToArray();

// Jeden konzument dequeueuje souběžně s producenty
var consumer = Task.Run(async () =>
{
    while (consumed < 12)   // celkem 3 × 4 zpráv
    {
        if (queue.TryDequeue(out string? msg))
        {
            Console.WriteLine($"  ← {msg}");
            Interlocked.Increment(ref consumed);
        }
        else
        {
            await Task.Delay(5);   // fronta prázdná – krátce počkej
        }
    }
});

await Task.WhenAll(producers.Append(consumer));
Console.WriteLine($"  Produkováno: {produced}, Spotřebováno: {consumed}");

// ── ConcurrentStack a ConcurrentBag ──────────────────────────────────────
// ConcurrentStack<T>: LIFO, lock-free. API: Push, TryPop, TryPeek.
//   Použití: undo history, depth-first work stealing.
//
// ConcurrentBag<T>: neuspořádaná kolekce, optimalizovaná pro scénáře kde
//   stejné vlákno přidává i odebírá (per-thread local list → méně contention).
//   Použití: object pooling, sběr výsledků z paralelních výpočtů.
