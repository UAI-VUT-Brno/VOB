using System.Collections.Frozen;
using System.Collections.Immutable;

// ImmutableCollections + FrozenDictionary – thread-safe kolekce jen pro čtení
// dotnet run --project 13-ImmutableCollections

// ── ImmutableList<T> ─────────────────────────────────────────────────────
// Add/Remove/SetItem vrátí NOVOU kolekci – původní nedotčena.
// Persistent data structure: sdílí uzly se starou verzí → Add je O(log n), ne O(n).
// Sdílená reference na ImmutableList nevyžaduje synchronizaci při čtení.
// Aktualizace sdílené reference: stejný vzor jako v 12-ImmutableSharedState.
//
// Dávkové vytváření → Builder: O(n) místo O(n log n) za n voláních Add.

var builder = ImmutableList.CreateBuilder<string>();
foreach (var c in new[] { "Praha", "Brno", "Ostrava", "Plzeň" }) builder.Add(c);
ImmutableList<string> list = builder.ToImmutable();

var list2 = list.Add("Liberec");
Console.WriteLine($"list:  [{string.Join(", ", list)}]");     // Praha, Brno, Ostrava, Plzeň
Console.WriteLine($"list2: [{string.Join(", ", list2)}]");    // + Liberec
Console.WriteLine($"list beze změny: {list.Count} prvků");    // stále 4

// ── FrozenDictionary<K,V> (.NET 8+) ──────────────────────────────────────
// Inicializovat jednou (při startu) → číst milionykrát.
// CLR při volání ToFrozenDictionary() analyzuje klíče a zvolí
// optimální hashovací strategii → nejrychlejší lookup ze všech slovníků v .NET.
// Nelze vůbec modifikovat (na rozdíl od ImmutableDictionary).
//
// Kdy použít: HTTP route table, country codes, feature flags, config lookup.

FrozenDictionary<string, string> countries = new Dictionary<string, string>
{
    ["CZ"] = "Česká republika", ["SK"] = "Slovensko",
    ["DE"] = "Německo",        ["AT"] = "Rakousko",
}.ToFrozenDictionary();

Console.WriteLine($"CZ → {countries["CZ"]}");
Console.WriteLine($"ContainsKey SK: {countries.ContainsKey("SK")}");

// ── Kdy co použít ─────────────────────────────────────────────────────────
// ImmutableList/Dictionary  → "mutace" přes snapshoty, verzování stavu, CQRS
// FrozenDictionary/Set      → read-only lookup tabulky, inicializovány jednou
// ConcurrentDictionary      → skutečně souběžné čtení i zápis za běhu (viz 07)
