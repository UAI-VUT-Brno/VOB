namespace Greenhouse.Equipment
{
    public class Thermometer
    {
        public double Temperature;        // C

        public void Measure(double value)
        {
            Temperature = value;
        }

        public void Print()
        {
            Console.WriteLine("temperature: " + Temperature + " C");
        }
    }
}
