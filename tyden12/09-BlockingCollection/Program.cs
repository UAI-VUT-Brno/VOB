// BlockingCollection<T> – ohraničená fronta s back-pressure
// dotnet run --project 09-BlockingCollection

// ConcurrentQueue nemá žádný limit – producent může zahltit paměť.
// BlockingCollection obaluje ConcurrentQueue (nebo jinou IProducerConsumerCollection)
// a přidává dvě klíčové vlastnosti:
//
//   BoundedCapacity:  Add() BLOKUJE vlákno když je fronta plná → přirozený back-pressure
//   CompleteAdding(): signalizuje konec produkce → GetConsumingEnumerable() se ukončí
//
// GetConsumingEnumerable():
//   • blokuje na prázdné frontě (nespotřebovává CPU)
//   • automaticky skončí až jsou splněny obě podmínky: CompleteAdding() + prázdná fronta
//   • vhodné pro synchronní pipelines (Thread-based, NE async/await)
//
// Pro async variantu s await foreach použijte Channel<T>.

using var pipeline = new System.Collections.Concurrent.BlockingCollection<string>(boundedCapacity: 3);

var producer = Task.Run(() =>
{
    try
    {
        for (int i = 1; i <= 8; i++)
        {
            pipeline.Add($"položka-{i:D2}");    // blokuje pokud je fronta plná
            Console.WriteLine($"  [P] vloženo: {i:D2}  (fronta: {pipeline.Count}/{pipeline.BoundedCapacity})");
            Thread.Sleep(20);
        }
    }
    finally
    {
        pipeline.CompleteAdding();    // ✓ vždy v finally – i při výjimce producenta
    }
});

var consumer = Task.Run(() =>
{
    foreach (var item in pipeline.GetConsumingEnumerable())
    {
        Console.WriteLine($"  [C] zpracováno: {item}");
        Thread.Sleep(50);    // konzument je pomalejší → back-pressure aktivní
    }
    Console.WriteLine("  [C] hotovo – CompleteAdding signalizoval konec");
});

Task.WhenAll(producer, consumer).GetAwaiter().GetResult();
