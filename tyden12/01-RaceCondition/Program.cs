// Race condition – data race a atomicita
// dotnet run --project 01-RaceCondition

// counter++ vypadá jako jedna operace, ale překládá se na:
//   READ  – načti hodnotu do registru
//   ADD   – přičti 1
//   WRITE – zapiš zpět do paměti
// Pokud jiné vlákno vstoupí mezi READ a WRITE, přepíše mezivýsledek → ztracený zápis.

int counter = 0;

var tasks = Enumerable.Range(0, 8)
    .Select(_ => Task.Run(() =>
    {
        for (int i = 0; i < 10_000; i++)
            counter++;   // ❌ READ-ADD-WRITE: tři kroky, ne jeden atomický
    }))
    .ToArray();

await Task.WhenAll(tasks);

Console.WriteLine($"Očekáváno: 80 000");
Console.WriteLine($"Skutečně:  {counter}  ← nedeterministický výsledek");

// ── Co je atomické, co není ───────────────────────────────────────────────────
//
// ATOMICKÉ (jedna instrukce, nelze přerušit):
//   • přiřazení int, bool, reference (na 64bitovém systému i long/double)
//   • čtení výše uvedených typů
//
// NENÍ atomické:
//   • counter++              (READ + ADD + WRITE)
//   • counter += n           (totéž)
//   • podmíněná aktualizace: if (x == 0) x = 1;  (read + branch + write)
//   • aktualizace dvou proměnných najednou
//   • list.Add(item)         (kontrola kapacity → možný resize → kopírování pole → zápis)
//   • dict[key] = value      (hash výpočet → slot → možný rehash celé tabulky)
//   → jakákoli operace nad List<T>, Dictionary<K,V> a dalšími mutable kolekcemi
//
// Řešení:
//   lock (gate) { counter++; }           → serialized přístup (viz 02-Lock)
//   Interlocked.Increment(ref counter)   → CPU-level atomická instrukce (viz 04-Interlocked)
//   immutabilní design                   → žádný sdílený mutable stav
