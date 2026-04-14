namespace Patterns.Core;

public class TemperatureSensor
{
    int _currentTemp = 100;

    public event Action<int> notify;

    public void NewTemperature()
    {
        _currentTemp  = new Random().Next(20, 200);
        notify?.Invoke(_currentTemp);
    }
}
