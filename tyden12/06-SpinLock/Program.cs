// SpinLock – aktivní čekání pro mikro-kritické sekce
// dotnet run --project 06-SpinLock

// lock (Monitor) při čekání předá vlákno OS scheduleru: vlákno usne, CPU jádro
// se přidělí jinému vláknu, po uvolnění zámku OS vlákno opět probudí (~1 µs overhead).
//
// SpinLock "aktivně čeká" (busy-wait): vlákno zůstane na CPU a v těsné smyčce
// opakovaně testuje, zda je zámek volný:
//   while (locked) { /* jen testuj, nic jiného */ }
// Žádný kernel overhead – ale vlákno spotřebovává 100 % CPU jádra i když jen čeká.
// Výhodné jen pokud čekání trvá kratší dobu než samotný context switch.
//
// Kdy SpinLock místo lock:
//   ✓  kritická sekce < ~20 CPU instrukcí (např. jen inkrementace)
//   ✓  vysoká frekvence přístupu, vícejádrový systém
//   ❌  delší sekce nebo async/await uvnitř → plýtvání CPU (100% utilization při čekání)
//
// POZOR: SpinLock je struct – nesmí se kopírovat! Vždy předávat ref.

var spinLock = new SpinLock(enableThreadOwnerTracking: false);
int counter = 0;

var threads = Enumerable.Range(0, 4)
    .Select(_ => new Thread(() =>
    {
        for (int i = 0; i < 5_000; i++)
        {
            bool taken = false;
            try
            {
                spinLock.Enter(ref taken);
                counter++;                        // kritická sekce – jen inkrementace
            }
            finally
            {
                if (taken) spinLock.Exit(useMemoryBarrier: false);
            }
        }
    }))
    .ToArray();

foreach (var t in threads) t.Start();
foreach (var t in threads) t.Join();

Console.WriteLine($"SpinLock: {counter} (očekáváno 20 000)");

// Rozhodovací strom:
//   jeden int čítač / příznak       → Interlocked (nejrychlejší)
//   mikro-sekce < 20 instrukcí      → SpinLock
//   obecná kritická sekce           → lock / Lock.EnterScope()
//   read-heavy workload             → ReaderWriterLockSlim
