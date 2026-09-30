// VOB - Exercise 3, block 3.1 - polymorphism
// One list, several kinds of power consumer, one loop.

namespace Oop0305
{
    // anything that draws electric power
    public interface IPowerConsumer
    {
        float PowerDraw();            // kW
    }

    public class Motor : IPowerConsumer
    {
        public float RatedPower;      // kW

        public float PowerDraw()
        {
            return RatedPower;
        }
    }

    // your Heater and Conveyor go here

    class Program
    {
        static void Main()
        {
            List<IPowerConsumer> line = new List<IPowerConsumer>
            {
                new Motor { RatedPower = 7.5f },
                new Motor { RatedPower = 3.0f },
            };

            float total = 0f;

            foreach (IPowerConsumer c in line)
            {
                Console.WriteLine(c.GetType().Name + ": " + c.PowerDraw() + " kW");
                total = total + c.PowerDraw();
            }

            Console.WriteLine("total: " + total + " kW");
        }
    }
}
