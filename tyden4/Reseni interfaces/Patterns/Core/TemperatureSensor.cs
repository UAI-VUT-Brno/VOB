namespace Patterns.Core;

public class TemperatureSensor : IObservable
{
    int _currentTemp = 100;
    List<IObserver> _observers = new();

    public void NewTemperature()
    {
        _currentTemp  = new Random().Next(20, 200);
        NotifyObservers();
    }

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }
    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach (IObserver observer in _observers)
        {
            observer.Update(_currentTemp);
        }
    }
}
