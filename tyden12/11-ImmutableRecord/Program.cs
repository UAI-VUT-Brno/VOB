// Immutabilita jako design volba – nejjednodušší cesta k thread-safety
// dotnet run --project 11-ImmutableRecord

// Race condition nastane při SOUBĚŽNÉM ČTENÍ A ZÁPISU.
// Pokud všechna vlákna pouze čtou → žádná synchronizace nutná.
// Immutabilní objekt = po vytvoření ho nelze změnit → vždy jen čtenáři.

// ── record – přirozeně immutabilní ───────────────────────────────────────
// record generuje poziční konstruktor, init-only vlastnosti, Equals a ToString.

var config = new ServerConfig("api.example.com", 443, TimeSpan.FromSeconds(30));

// Souběžné čtení z libovolného počtu vláken – žádný lock potřeba
await Task.WhenAll(Enumerable.Range(0, 100).Select(_ =>
    Task.Run(() => { string _ = $"{config.Host}:{config.Port}"; })));

Console.WriteLine($"Config: {config}");
Console.WriteLine("Žádná synchronizace – objekt je immutabilní.");

// ── with-výraz: „změna" bez mutace ───────────────────────────────────────
// with vytvoří novou kopii s pozměněnými hodnotami – původní objekt nedotčen.
// Ostatní vlákna stále drží referenci na starý objekt → nevidí žádnou změnu.
// Jak správně sdílet aktualizovanou referenci s ostatními vlákny → viz 12-ImmutableSharedState.

var updated = config with { Port = 8080 };
Console.WriteLine($"Původní port: {config.Port}   Nový port: {updated.Port}");
Console.WriteLine($"Různé instance: {!ReferenceEquals(config, updated)}");

// ── init – vlastnost nastavitelná jen při inicializaci ───────────────────
// Kompilátorem vynucená immutabilita – po vytvoření objektu nelze přiřadit.

var order = new Order { Id = Guid.NewGuid(), Total = 299.99m };
// order.Total = 0;   // ❌ CS8852 – init-only property nelze přiřadit po inicializaci
Console.WriteLine($"Objednávka {order.Id} = {order.Total:C}");

// ── Supporting types ──

record ServerConfig(string Host, int Port, TimeSpan Timeout);

record Order
{
    public Guid    Id    { get; init; }
    public decimal Total { get; init; }
}
