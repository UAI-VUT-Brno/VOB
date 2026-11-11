using CsvExport.Devices;

namespace CsvExport
{
    class Program
    {
        static void Main()
        {
            List<Pump> pumps = new List<Pump>
            {
                new Pump { Name = "Pump-1", FlowRate = 12.5, Running = true },
                new Pump { Name = "Pump-2", FlowRate = 8.4, Running = false },
                new Pump { Name = "Pump-3", FlowRate = 15.0, Running = true }
            };
            List<Fan> fans = new List<Fan>
            {
                new Fan { Name = "Fan-A", Speed = 1450, Power = 0.75 },
                new Fan { Name = "Fan-B", Speed = 2900, Power = 1.5 }
            };
        }
    }
}
