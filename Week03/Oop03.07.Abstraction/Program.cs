// VOB - Exercise 3, block 4.1 - abstraction
// The thermometer measures. Where the number ends up is not its business.

namespace Oop0307
{
    public class ConsoleDisplay
    {
        public void Show(string text)
        {
            Console.WriteLine("[screen] " + text);
        }
    }

    public class Thermometer
    {
        private ConsoleDisplay _display;

        public Thermometer(ConsoleDisplay display)
        {
            _display = display;
        }

        public void Measure(double temperature)
        {
            _display.Show("temperature " + temperature + " C");
        }
    }

    class Program
    {
        static void Main()
        {
            Thermometer t = new Thermometer(new ConsoleDisplay());
            t.Measure(21.5);
        }
    }
}
