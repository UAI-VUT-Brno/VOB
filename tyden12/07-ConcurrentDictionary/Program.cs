// ConcurrentDictionary<K,V> – thread-safe slovník
// dotnet run --project 07-ConcurrentDictionary

// Dictionary<K,V> NENÍ thread-safe. Paralelní zápisy způsobují:
//   • InvalidOperationException při interním resize/rehash
//   • poškozené vnitřní struktury (nekonečná smyčka při čtení)
//   • tichou ztrátu dat
// Ani souběžné čtení NENÍ bezpečné pokud zároveň probíhá zápis.

// ── Dictionary + lock vs ConcurrentDictionary ────────────────────────────
// Dictionary + jeden lock: operace jsou serializovány – provádějí se striktně jedna
// po druhé, i když každá pracuje s jiným klíčem a mohly by bezpečně běžet paralelně.
// Při 20 vláknech čeká 19 z nich, i když každé sahá na zcela jiný klíč.
// ConcurrentDictionary: fine-grained locking (16 segmentů) –
//   souběžné zápisy do různých segmentů probíhají skutečně paralelně.
//   Čtení existujících hodnot je lock-free.

var cd = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

// ── AddOrUpdate – atomické přidat nebo aktualizovat ──────────────────────
// Garantuje že mezi čtením a zápisem nevstoupí jiné vlákno.
// updateValueFactory může být volána vícekrát při souběhu – musí být idempotentní!

string[] words = ["apple", "banana", "apple", "cherry", "banana", "apple"];
await Task.WhenAll(words.Select(w => Task.Run(() =>
    cd.AddOrUpdate(w, addValue: 1, updateValueFactory: (_, n) => n + 1))));

foreach (var (w, n) in cd.OrderBy(kv => kv.Key))
    Console.WriteLine($"  {w}: {n}");

// ── GetOrAdd – získat nebo přidat ─────────────────────────────────────────
// POZOR: valueFactory NENÍ chráněna lockem –
//   při souběhu může proběhnout vícekrát, do slovníku se uloží první výsledek.
//   Pokud je factory drahá nebo má side-efekty, obalte ji do Lazy<T>.

var cache = new System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<string>>();
string result = cache
    .GetOrAdd("cfg", _ => new Lazy<string>(() =>
    {
        Console.WriteLine("  [factory] volána jen jednou, i při souběhu");
        return "production-config";
    }))
    .Value;

Console.WriteLine($"  cache: {result}");

// ── TryGetValue / TryRemove / TryUpdate ──────────────────────────────────
// Všechny Try* operace jsou atomické a nikdy nevyhazují výjimku.
// TryUpdate(key, newValue, comparisonValue) – aktualizuje jen pokud stávající == comparisonValue.

cd.TryGetValue("apple", out int appleCount);
Console.WriteLine($"  apple count (TryGet): {appleCount}");

bool removed = cd.TryRemove("cherry", out _);
Console.WriteLine($"  cherry odstraněno: {removed},  zbývá: {cd.Count} klíčů");
