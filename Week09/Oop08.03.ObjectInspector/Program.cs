using ObjectInspection.Devices;

namespace ObjectInspection
{
    class Program
    {
        static void Main()
        {
            Pump pump = new Pump { Name = "Pump-1", FlowRate = 12.5, Running = true };
            Heater heater = new Heater { Name = "Heater-2", Power = 2.5, Temperature = 64.0 };
        }
    }
}
