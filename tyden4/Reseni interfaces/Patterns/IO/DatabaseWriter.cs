using Patterns.Core;

namespace Patterns.IO;

public class DatabaseWriter: IObserver
{
    public void Update(int newPrice)
    {
            Console.WriteLine($"Temperature {newPrice} added to new line in database.");
    }
}

