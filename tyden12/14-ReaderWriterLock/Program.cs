// ReaderWriterLockSlim – souběžné čtení, exkluzivní zápis
// dotnet run --project 14-ReaderWriterLock

// Problém s plain lock: i souběžné čtení je blokováno.
// Při 100 čtenářích a 1 zapisovateli lock zbytečně serializuje vše.
//
// ReaderWriterLockSlim (RWLS):
//   EnterReadLock()             – sdílený přístup; blokuje jen pokud píše zapisovatel
//   EnterWriteLock()            – exkluzivní; blokuje dokud skončí všichni čtenáři
//   EnterUpgradeableReadLock()  – lze upgradovat na write bez uvolnění read locku
//
// POZOR: nesmí await uvnitř locku – RWLS není kompatibilní s async/await!
// Pro async read-heavy scénáře použijte ConcurrentDictionary nebo immutabilní design.

using var cache = new ReadHeavyCache();

var readers = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
{
    for (int j = 0; j < 5; j++) _ = cache.Get($"k{i % 3}");
}));
var writers = new[]
{
    Task.Run(() => cache.Set("k0", "Praha")),
    Task.Run(() => cache.Set("k1", "Brno")),
};
await Task.WhenAll(readers.Concat(writers));
Console.WriteLine($"Cache count: {cache.Count}");

// ── UpgradeableReadLock – check-then-act ─────────────────────────────────
// Scénář: přečíst (read lock) → pokud chybí, zapsat (write lock).
// Naivně: exit read → enter write → jiné vlákno stihne vložit mezitím → duplicita.
// UpgradeableReadLock: vlákno čte a může upgradovat na write atomicky.
// Pouze jedno vlákno smí držet upgradeable lock najednou.

string city = cache.GetOrAdd("k2", () => "Ostrava");
Console.WriteLine($"GetOrAdd k2: {city}");

// ── Supporting types ──

sealed class ReadHeavyCache : IDisposable
{
    private readonly ReaderWriterLockSlim _rwls = new();
    private readonly Dictionary<string, string> _store = [];

    public string? Get(string key)
    {
        _rwls.EnterReadLock();
        try   { return _store.GetValueOrDefault(key); }
        finally { _rwls.ExitReadLock(); }
    }

    public void Set(string key, string value)
    {
        _rwls.EnterWriteLock();
        try   { _store[key] = value; }
        finally { _rwls.ExitWriteLock(); }
    }

    public string GetOrAdd(string key, Func<string> factory)
    {
        _rwls.EnterUpgradeableReadLock();
        try
        {
            if (_store.TryGetValue(key, out var v)) return v;
            _rwls.EnterWriteLock();
            try
            {
                if (_store.TryGetValue(key, out v)) return v;   // double-check po upgrade
                return _store[key] = factory();
            }
            finally { _rwls.ExitWriteLock(); }
        }
        finally { _rwls.ExitUpgradeableReadLock(); }
    }

    public int Count
    {
        get { _rwls.EnterReadLock(); try { return _store.Count; } finally { _rwls.ExitReadLock(); } }
    }

    public void Dispose() => _rwls.Dispose();
}
