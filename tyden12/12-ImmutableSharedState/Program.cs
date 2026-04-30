// Sdílená immutabilní reference – aneb "stale" reference a jak se jí vyhnout
// dotnet run --project 12-ImmutableSharedState

// Immutabilní objekt sám o sobě nevyžaduje synchronizaci při čtení.
// Ale SDÍLENÁ REFERENCE na něj ji vyžaduje při aktualizaci.
//
// Problém (stale reference):
//   AppConfig config = AppConfig.Default;              // sdílená reference
//
//   Vlákno A: config = config with { Timeout = 60 };  // zamění referenci
//   Vlákno B: config.Timeout                          // může přečíst starou
//                                                     // hodnotu z CPU cache!
//
// Přiřazení reference sice atomické, ale bez memory barrier CPU cache
// jiného vlákna nevidí novou hodnotu – to způsobuje "stale" read.

// ── Řešení 1: Volatile.Write / Volatile.Read (jeden zapisovatel) ─────────
// Volatile.Write  → zapíše a zaručí propagaci do sdílené paměti (memory barrier)
// Volatile.Read   → zaručí načtení přímo z paměti, ne z CPU cache

AppConfig sharedConfig = AppConfig.Default;

var reloader = Task.Run(async () =>
{
    await Task.Delay(25);
    var snap    = Volatile.Read(ref sharedConfig);
    var updated = snap with { Timeout = 60 };
    Volatile.Write(ref sharedConfig, updated);   // ✓ ostatní vlákna uvidí novou hodnotu
    Console.WriteLine($"  [reload] Timeout → {updated.Timeout}s");
});

var readers = Enumerable.Range(0, 4).Select(i => Task.Run(async () =>
{
    await Task.Delay(i * 15);
    var snapshot = Volatile.Read(ref sharedConfig);   // ✓ čerstvé čtení bez cache
    Console.WriteLine($"  [reader {i}] Timeout = {snapshot.Timeout}s");
}));

await Task.WhenAll(readers.Append(reloader));

// ── Řešení 2: Interlocked.CompareExchange – více zapisovatelů ────────────
// CAS loop: přečti snapshot → vytvoř nový objekt → atomicky nahraď referenci
// jen pokud snapshot stále odpovídá aktuální hodnotě.
// Pokud mezitím zapsal jiný vlákno → CAS selže → opakuj s čerstvým snapshoten.
// Každá aktualizace je tak zachována, žádná se neztratí.

Console.WriteLine();
AppConfig current = AppConfig.Default;

await Task.WhenAll(Enumerable.Range(1, 5).Select(_ => Task.Run(() =>
{
    AppConfig snap, next;
    do
    {
        snap = Volatile.Read(ref current);
        next = snap with { Version = snap.Version + 1 };
    }
    while (Interlocked.CompareExchange(ref current, next, snap) != snap);
    // Pokud jiné vlákno stihlo zapsat dříve → CompareExchange vrátí jinou hodnotu než snap
    // → podmínka while je true → retry s čerstvým snapshotem → žádný zápis se neztratí
})));

Console.WriteLine($"  Verze po 5 paralelních aktualizacích: {current.Version}  (vždy 5)");

// ── Supporting types ──

record AppConfig(int Timeout, int Version)
{
    public static readonly AppConfig Default = new(Timeout: 30, Version: 0);
}
