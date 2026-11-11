using ConveyorLine.Models;

namespace ConveyorLine.Machines
{
    public class Conveyor
    {
        // raised for every item that leaves the belt
        public event Action<Item> ItemPassed;

        public void Run(List<Item> items)
        {
            foreach (Item item in items)
            {
                ItemPassed?.Invoke(item);
            }
        }
    }
}
