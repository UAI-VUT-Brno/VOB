using Patterns.Core;
using Patterns.UI;
using Patterns.IO;

namespace Patterns;
class Program
{
    static void Main()
    {
        Display display = new Display();
        DatabaseWriter dbWriter = new DatabaseWriter();

        TemperatureSensor temperatureSensor  = new TemperatureSensor();
        temperatureSensor.notify += display.Update;
        temperatureSensor.notify += dbWriter.Update;

        TemperatureDataStreamSim(temperatureSensor); //placeholder for internal event
        
        temperatureSensor.notify -= display.Update; //unsubscribe when not needed
        temperatureSensor.notify -= dbWriter.Update;
        
        Console.ReadKey();
    }

    static void TemperatureDataStreamSim(TemperatureSensor temperatureSensor)
    {
        temperatureSensor.NewTemperature(); 
        temperatureSensor.NewTemperature(); 
        temperatureSensor.NewTemperature(); 
    }
}
