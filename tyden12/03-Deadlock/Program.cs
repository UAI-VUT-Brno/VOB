// Deadlock – příčina a prevence
// dotnet run --project 03-Deadlock

// Deadlock nastane při cyklické závislosti na zámky:
//   Vlákno A: lock(lockA) { lock(lockB) { ... } }
//   Vlákno B: lock(lockB) { lock(lockA) { ... } }   ← opačné pořadí!
//
// Pokud A drží lockA a B drží lockB, obě čekají věčně → program visí.
// Coffmanovy podmínky (musí platit všechny zároveň):
//   1. Mutual exclusion – zdroj drží jen jedno vlákno
//   2. Hold and wait    – vlákno drží zdroj a čeká na další
//   3. No preemption    – zdroj nelze odebrat násilím
//   4. Circular wait    – cyklická závislost v čekání

// ── Prevence 1: konzistentní pořadí zámků ────────────────────────────────
// Pravidlo: vždy zamykat ve stejném pořadí → circular wait (podmínka 4) nenastane.

object lockA = new(), lockB = new();

var t1 = new Thread(() => { lock (lockA) { lock (lockB) { Console.WriteLine("T1 hotovo"); } } });
var t2 = new Thread(() => { lock (lockA) { lock (lockB) { Console.WriteLine("T2 hotovo"); } } });
// ✓ Obě vlákna: lockA → lockB (stejné pořadí, deadlock nehrozí)
t1.Start(); t2.Start(); t1.Join(); t2.Join();

// ── Prevence 2: Monitor.TryEnter s timeoutem ─────────────────────────────
// TryEnter vrátí false místo věčného čekání → aplikace může situaci řešit.
// Vhodné pokud pořadí zámků nelze zaručit (cizí knihovny, složité grafy objektů).

bool gotA = Monitor.TryEnter(lockA, TimeSpan.FromMilliseconds(200));
if (gotA)
{
    try
    {
        bool gotB = Monitor.TryEnter(lockB, TimeSpan.FromMilliseconds(200));
        if (gotB)
        {
            try   { Console.WriteLine("TryEnter: oba zámky získány"); }
            finally { Monitor.Exit(lockB); }
        }
        else { Console.WriteLine("TryEnter: lockB nedostupný – retry nebo přerušit"); }
    }
    finally { Monitor.Exit(lockA); }
}
