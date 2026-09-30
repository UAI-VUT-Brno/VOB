// VOB - Exercise 3, block 2.2 - an interface, or a parent class?
// Everything in the hall needs a yearly inspection - the extinguishers too.

namespace Oop0304
{
    public class Machine
    {
        public string Name;
        public bool Running;

        public void Start()
        {
            Running = true;
            Console.WriteLine(Name + " started");
        }

        public void Stop()
        {
            Running = false;
            Console.WriteLine(Name + " stopped");
        }
    }

    public class Lathe : Machine
    {
        public float MaxDiameter;     // mm
    }

    public class Mill : Machine
    {
        public int Axes;
    }

    public class Extinguisher
    {
        public string Location;
        public float Kilograms;
    }

    class Program
    {
        // static void Report(??? item)
        // {
        //     Console.WriteLine("[inspection] " + item.Inspect());
        // }

        static void Main()
        {
            Lathe lathe = new Lathe { Name = "L-1", MaxDiameter = 400 };
            Mill mill = new Mill { Name = "F-2", Axes = 5 };
            Extinguisher ext = new Extinguisher { Location = "gate 2", Kilograms = 6 };

            // Report(lathe);
            // Report(mill);
            // Report(ext);
        }
    }
}
