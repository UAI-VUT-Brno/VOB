using Greenhouse.Equipment;
using Greenhouse.Plants;

namespace Greenhouse
{
    class Program
    {
        static void Main()
        {
            Plant tomato = new Plant("tomato");
            Plant basil = new Plant("basil");

            Thermometer thermometer = new Thermometer();
            Sprinkler sprinkler = new Sprinkler(30);

            thermometer.Measure(27.5);
            thermometer.Print();

            sprinkler.Water(tomato);
            sprinkler.Water(tomato);
            sprinkler.Water(basil);

            tomato.Print();
            basil.Print();
        }
    }
}
