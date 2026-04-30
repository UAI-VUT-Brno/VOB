// Lazy<T>, ThreadLocal<T>, AsyncLocal<T>
// dotnet run --project 15-LazyAndThreadLocal

// ── Lazy<T> – thread-safe inicializace na první přístup ──────────────────
// Výchozí mode ExecutionAndPublication: továrna zavolána jednou, výsledek sdílen.
// IsValueCreated: lze zkontrolovat bez spuštění inicializace.
// Reálný příklad: drahé připojení k DB, konfigurace z disku, singleton service.

var lazyDb = new Lazy<string>(() =>
{
    Console.WriteLine("  [INIT] Inicializace DB připojení...");
    return "Server=localhost;Database=shop";
});

Console.WriteLine($"  IsValueCreated: {lazyDb.IsValueCreated}");
await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => Task.Run(() => { string __ = lazyDb.Value; })));
Console.WriteLine($"  Connection: {lazyDb.Value}");    // [INIT] proběhlo jen jednou

// ── ThreadLocal<T> – každé vlákno má svou vlastní instanci ───────────────
// Vlákna nesdílí stav → žádná synchronizace nutná.
// Klasický případ: Random (není thread-safe), StringBuilder, per-thread buffer.
// trackAllValues: true → Values property vrátí všechny live instance pro agregaci.

using var rng = new ThreadLocal<Random>(
    () => new Random(Thread.CurrentThread.ManagedThreadId),
    trackAllValues: true);

var threads = Enumerable.Range(0, 3).Select(_ => new Thread(() =>
    Console.WriteLine($"  Thread [{Thread.CurrentThread.ManagedThreadId}]: {rng.Value!.Next(100)}")
)).ToArray();
foreach (var t in threads) { t.Start(); t.Join(); }
Console.WriteLine($"  Instancí Random: {rng.Values.Count}");

// ── AsyncLocal<T> – stav přes async call chain ───────────────────────────
// ThreadLocal váže hodnotu na vlákno – po await může pokračování běžet na jiném vlákně.
// AsyncLocal váže hodnotu na ExecutionContext (logické vlákno) → přežije await.
//
// Hodnota nastavená rodičem je viditelná v potomcích.
// Změna v potomkovi NENÍ viditelná v rodiči (copy-on-write sémantika ExecutionContext).
// Reálný příklad: correlation ID, tenant ID, user identity – bez parametru přes celý stack.

var correlationId = new AsyncLocal<string?>();
correlationId.Value = "req-abc";

await Task.Run(async () =>
{
    Console.WriteLine($"  [child]  {correlationId.Value}");   // zděděno
    correlationId.Value = "req-child";
    await Task.Delay(1);
    Console.WriteLine($"  [child po await]  {correlationId.Value}");   // stále platí
});

Console.WriteLine($"  [rodič]  {correlationId.Value}");   // nezměněno – copy-on-write
