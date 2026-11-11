using ConveyorLine.Machines;
using ConveyorLine.Models;

namespace ConveyorLine
{
    class Program
    {
        static void Main()
        {
            Conveyor conveyor = new Conveyor();
            List<Item> batch = new List<Item>
            {
                new Item("gearbox", 18.5),
                new Item("motor", 24.0),
                new Item("shaft", 6.2),
                new Item("housing", 31.7)
            };

            conveyor.Run(batch);
        }
    }
}
