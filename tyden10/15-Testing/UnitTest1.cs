// Slide 19: Testování async kódu (MSTest)
// Spusť: dotnet test examples/15-Testing

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncAwaitExamples.Testing;

[TestClass]
public class AsyncExampleTests
{
    // ✅ SPRÁVNĚ: async Task – MSTest awaits správně
    [TestMethod]
    public async Task GetLength_ReturnsPositiveValue()
    {
        int length = await GetLengthAsync();
        Assert.IsTrue(length > 0);
    }

    // ✅ Testování výjimek z async kódu
    // Task.Delay vyhazuje TaskCanceledException (: OperationCanceledException)
    [TestMethod]
    public async Task DoWork_WhenCanceled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // ThrowsExceptionAsync: V MSTest testujeme konkrétní typ TaskCanceledException
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(
            () => DoWorkAsync(cts.Token));
    }

    // ✅ Timeout pro testy – nesmí zasekávat CI pipeline
    [TestMethod]
    [Timeout(5000)]
    public async Task LongOperation_CompletesInTime()
    {
        await DoWorkAsync(CancellationToken.None);
    }

    // ✅ Paralelní test – WhenAll
    [TestMethod]
    public async Task WhenAll_AggregatesResults()
    {
        Task<int>[] tasks = [GetLengthAsync(), GetLengthAsync(), GetLengthAsync()];
        int[] results = await Task.WhenAll(tasks);
        Assert.AreEqual(3, results.Length);
        CollectionAssert.AllItemsAreNotNull(results);
        foreach (var r in results)
        {
            Assert.IsTrue(r > 0);
        }
    }

    // ⚠️  ŠPATNĚ (záměrně zakomentováno): async void test
    // MSTest to nepodporuje; test falešně projde
    //
    // [TestMethod]
    // public async void WrongVoidTest()
    // {
    //     int length = await GetLengthAsync();
    //     Assert.IsTrue(length > 0);   // toto se NEMUSÍ vykonat
    // }

    private static async Task<int> GetLengthAsync()
    {
        await Task.Delay(10);
        return 42;
    }

    private static async Task DoWorkAsync(CancellationToken ct)
    {
        await Task.Delay(100, ct);
    }
}