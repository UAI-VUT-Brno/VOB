using Patterns.Core;

namespace Patterns.UI;

public class Display : IObserver
{
    readonly int _threshold = 110;

    public void Update(int temperature)
    {
        if (temperature > _threshold)
        {
            Console.WriteLine($"Warning: Temperature {temperature} exceeds safety threshold {_threshold}.");
        }
    }
}
