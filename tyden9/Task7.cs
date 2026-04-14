Thread thread1 = new Thread(Task7.PrintNumbers);
Thread thread2 = new Thread(Task7.PrintCharacters);

thread1.Start();
thread2.Start();

Console.WriteLine($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}");

public class Task7
{
    
    static void PrintNumbers()
    {
        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        for (int i = 1; i <= 100; i++)
        {
            Console.WriteLine(i);
        }
    }

    static void PrintCharacters()
    {
        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        for (int i = 0; i < 100; i++)
        {
            char c = (char)('a' + i);
            Console.WriteLine(c);
        }
    }
}