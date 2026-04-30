// volatile – viditelnost paměti přes vlákna
// dotnet run --project 05-Volatile

// Bez volatile může JIT nebo CPU cachovat proměnnou do registru.
// Jiné vlákno pak vidí zastaralou hodnotu → příznak „stop" se nikdy nepropaguje.
//
// volatile zabrání této optimalizaci:
//   • každý čtecí přístup načte hodnotu přímo z paměti
//   • každý zápis okamžitě zapíše do paměti
//
// volatile NEZARUČUJE atomicitu operací READ-MODIFY-WRITE (++, +=)!
//   volatile bool flag   → OK (přiřazení bool je atomické)
//   volatile int counter → ŠPATNĚ pro counter++ (není atomické, viz 04-Interlocked)

var worker = new Worker();
var thread = new Thread(worker.Run);
thread.Start();

Thread.Sleep(50);
worker.RequestStop();   // nastaví volatile příznak
thread.Join(TimeSpan.FromSeconds(2));

Console.WriteLine($"Vlákno zastaveno: {!thread.IsAlive}");

// ── Volatile.Read / Volatile.Write ────────────────────────────────────────
// Alternativa k volatile klíčovému slovu pro jednotlivá čtení/zápisy.
// Vhodné pokud není třeba označit celé pole jako volatile, nebo při práci
// s polem prvků (arrays – volatile klíčové slovo na prvky pole nelze použít).
//
// Typické use cases pro volatile / Volatile.Read+Write:
//   • stop-flag pro vlákno (viz Worker výše)
//   • signalizace dokončení inicializace jiným vláknem (isReady = true)
//   • publikace immutabilního objektu: jeden pisatel, ostatní jen čtou (viz 12)
//   • low-level stavový příznak v real-time nebo embedded kódu
//
// volatile nestačí pokud:
//   • více vláken zapisuje (race condition zůstává) → Interlocked nebo lock
//   • invarianta přes více proměnných → lock

int sharedValue = 0;
Volatile.Write(ref sharedValue, 42);
Console.WriteLine($"Volatile.Read: {Volatile.Read(ref sharedValue)}");

// ── Supporting types ──

sealed class Worker
{
    private volatile bool _stopRequested;

    public void RequestStop() => _stopRequested = true;

    public void Run()
    {
        int iterations = 0;
        while (!_stopRequested)    // ✓ volatile zajistí čerstvé čtení
        {
            iterations++;
            Thread.SpinWait(100);
        }
        Console.WriteLine($"Worker skončil po {iterations} iteracích.");
    }
}
