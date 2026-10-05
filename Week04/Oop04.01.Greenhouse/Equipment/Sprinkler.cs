using Greenhouse.Plants;

namespace Greenhouse.Equipment
{
    public class Sprinkler
    {
        public int Dose;                  // ml at a time

        public Sprinkler(int dose)
        {
            Dose = dose;
        }

        public void Water(Plant plant)
        {
            plant.Water = plant.Water + Dose;
            Console.WriteLine("watering the " + plant.Name);
        }
    }
}
