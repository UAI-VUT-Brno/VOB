// Slide 04: Task a Task<T> – stavy a použití

using System;
using System.Threading.Tasks;

// Task bez výsledku
Task t1 = Task.Run(() => Console.WriteLine("Běžím na ThreadPoolu"));
Console.WriteLine($"Stav po spuštění: {t1.Status}");
await t1;
Console.WriteLine($"Stav po dokončení: {t1.Status}");

// Task<T> s výsledkem
Task<int> t2 = Task.Run(() => 42);
int result = await t2;
Console.WriteLine($"Výsledek: {result}");

// Task.CompletedTask a Task.FromResult – synchronně hotové (nealokují vlákno)
Task done = Task.CompletedTask;
Task<string> immediate = Task.FromResult("Hotovo ihned");
Console.WriteLine(await immediate);

// Ukázka Faulted stavu
Task faulted = Task.Run(() => { throw new InvalidOperationException("Boom"); });
try { await faulted; } catch { }
Console.WriteLine($"Faulted task stav: {faulted.Status}");
