// Slide 15: TaskCompletionSource – obálka callback API

using System;
using System.Threading;
using System.Threading.Tasks;

// Simulace starého callback API (Timer, Socket, P/Invoke…)
static void OldCallbackApi(int delayMs, Action<string> onSuccess, Action<Exception> onError)
{
    ThreadPool.QueueUserWorkItem(_ =>
    {
        Thread.Sleep(delayMs);

        if (delayMs < 1000)
            onSuccess($"Výsledek po {delayMs}ms");
        else
            onError(new TimeoutException("Příliš pomalé"));
    });
}

// TaskCompletionSource – "umožňuje nám vytvořit Task, který můžeme splnit/selhat z libovolného místa (např. z callbacku)"
// ✅ Obalíme callback API do Task pomocí TaskCompletionSource
static Task<string> WrapAsAsync(int delayMs)
{
    var tcs = new TaskCompletionSource<string>();

    OldCallbackApi(
        delayMs,
        onSuccess: result => tcs.SetResult(result),
        onError:   ex     => tcs.SetException(ex)
    );

    return tcs.Task;  // vrátíme Task, který se splní/selhá až callback zavolá Set*
}

// Použití – moderní await
try
{
    string result = await WrapAsAsync(200);
    Console.WriteLine($"✅ {result}");

    string slow = await WrapAsAsync(2000);  // selhá
    Console.WriteLine(slow);
}
catch (TimeoutException ex)
{
    Console.WriteLine($"❌ Timeout: {ex.Message}");
}
