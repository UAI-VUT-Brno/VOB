using Patterns.UI;
using Patterns.IO;
using Patterns.Core;

namespace Patterns;

class Program
{
    static void Main()
    {
        Display display = new Display();
        DatabaseWriter dbWriter = new DatabaseWriter();

        TemperatureSensor temperatureSensor = new TemperatureSensor(display, dbWriter);

        TemperatureDataStreamSim(temperatureSensor); //placeholder for internal event

        Console.ReadKey();
    }

    static void TemperatureDataStreamSim(TemperatureSensor temperatureSensor)
    {
        temperatureSensor.NewTemperature();
        temperatureSensor.NewTemperature();
        temperatureSensor.NewTemperature();
    }
}
