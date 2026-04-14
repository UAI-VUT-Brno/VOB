
using Patterns.UI;
using Patterns.IO;

namespace Patterns.Core;

public class TemperatureSensor
{
    int _currentTemp = 100;
    readonly Display _display;
    readonly DatabaseWriter _dbWriter;

    public TemperatureSensor(Display display, DatabaseWriter dbWriter)
    {
        _currentTemp = 100;
        _display = display;
        _dbWriter = dbWriter;
    }

    public void NewTemperature()
    {
        _currentTemp  = new Random().Next(20, 200);
        //notify others
        _display.Update(_currentTemp);
        _dbWriter.Update(_currentTemp);
    }
}
