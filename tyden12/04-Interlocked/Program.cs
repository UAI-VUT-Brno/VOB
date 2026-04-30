// Interlocked – atomické operace bez locku
// dotnet run --project 04-Interlocked

// Interlocked používá CPU instrukce LOCK XADD / LOCK CMPXCHG.
// Žádný kernel call, žádné čekání → výrazně rychlejší než lock pro jednoduché operace.
//
// Kdy použít Interlocked místo lock:
//   ✓  čítač přístupu (počet requestů, chyb, aktivních spojení)
//   ✓  globální příznak (isShuttingDown, isInitialized)
//   ✓  lock-free maximum, minimum, stavový automat s jednou proměnnou
//   ❌  invarianta přes více proměnných najednou → vyžaduje lock
//
// Proč Interlocked nestačí pro invariantu přes více proměnných:
//   Příklad: účet sleduje balance a počet transakcí zároveň.
//   Interlocked.Add(ref balance, -amount);        // atomické
//   Interlocked.Increment(ref transactionCount);  // atomické
//   Vlákno čtoucí obě hodnoty může narazit na okamžik, kdy
//   balance je snížen, ale transactionCount ještě ne → nekonzistentní snapshot.
//   Pro invariantu "obě hodnoty jsou vždy konzistentní spolu" je nutný lock
//   nebo immutabilní objekt sdílený přes CAS (viz 12-ImmutableSharedState).

// ── Increment / Decrement / Add ───────────────────────────────────────────
int counter = 0;

await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
{
    for (int i = 0; i < 10_000; i++)
        Interlocked.Increment(ref counter);   // ✓ atomické READ-ADD-WRITE v jedné instrukci
})));

Console.WriteLine($"Interlocked.Increment: {counter}");   // vždy 80 000

// ── Exchange a CompareExchange (CAS) ─────────────────────────────────────
// CompareExchange(ref location, newValue, comparand):
//   if (location == comparand) location = newValue;
//   return původní location
// Atomicky – základ všech lock-free algoritmů.

int value = 10;
int old = Interlocked.Exchange(ref value, 42);
Console.WriteLine($"Exchange: nová={value}, stará={old}");

// Optimistická aktualizace (CAS retry loop) – lock-free zdvojení hodnoty:
int snapshot, updated;
do
{
    snapshot = value;
    updated  = snapshot * 2;
}
while (Interlocked.CompareExchange(ref value, updated, snapshot) != snapshot);
Console.WriteLine($"CAS retry loop: {value}");   // 84

// Lazy init flag – inicializovat pouze jednou, i při souběhu:
int initialized = 0;
if (Interlocked.CompareExchange(ref initialized, 1, comparand: 0) == 0)
    Console.WriteLine("Inicializace proběhla (jen jednou).");
