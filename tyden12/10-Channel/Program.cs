// System.Threading.Channels – async producer-consumer pipeline
// dotnet run --project 10-Channel

// Channel<T> je moderní náhrada BlockingCollection pro async kód:
//   BlockingCollection.Add()     blokuje vlákno (synchronní)
//   channel.Writer.WriteAsync()  čeká asyncně (neblokuje vlákno)
//
//   BlockingCollection konzument: foreach (blokující vlákno)
//   Channel konzument:            await foreach přes ReadAllAsync() (IAsyncEnumerable)
//
// Channel.CreateBounded(n)   – back-pressure: WriteAsync čeká pokud plno
// Channel.CreateUnbounded()  – neomezená kapacita (pozor na paměť)

var channel = System.Threading.Channels.Channel.CreateBounded<string>(capacity: 3);

var producer = Task.Run(async () =>
{
    try
    {
        foreach (var city in new[] { "Praha", "Brno", "Ostrava", "Plzeň", "Liberec", "Olomouc" })
        {
            await channel.Writer.WriteAsync(city);   // asyncně čeká pokud fronta plná
            Console.WriteLine($"  [W] {city}");
            await Task.Delay(20);
        }
    }
    finally
    {
        channel.Writer.Complete();    // ✓ vždy v finally – signalizuje konec proudu
    }
});

var consumer = Task.Run(async () =>
{
    await foreach (var city in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine($"  [R] zpracováno: {city}");
        await Task.Delay(40);    // pomalejší konzument → back-pressure aktivní
    }
});

await Task.WhenAll(producer, consumer);

// ── Vícestupňová pipeline ─────────────────────────────────────────────────
// Channel<T> lze řetězit: zdroj → transformace → výstup.
// Každý stupeň je samostatný Task čtoucí z jednoho kanálu a zapisující do druhého.
// Kanály přirozeně zajišťují back-pressure mezi stupni.
// Reálný příklad: čtení CSV → parsování řádků → uložení do DB.
