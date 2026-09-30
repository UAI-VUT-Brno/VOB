// VOB - Exercise 3, block 2.1 - interfaces
// A robot, a conveyor and a scale have little in common,
// but some of them can do the same things.

namespace Oop0303
{
    public class Robot
    {
        public string Name;

        public void EmergencyStop()
        {
            Console.WriteLine(Name + ": arm stopped");
        }

        public void Calibrate()
        {
            Console.WriteLine(Name + ": axes zeroed");
        }
    }

    public class Conveyor
    {
        public string Name;

        public void EmergencyStop()
        {
            Console.WriteLine(Name + ": belt stopped");
        }
    }

    public class Scale
    {
        public string Name;

        public void Calibrate()
        {
            Console.WriteLine(Name + ": zero set");
        }
    }

    // your interfaces go here

    class Program
    {
        // static void EmergencyButton(IStoppable device)
        // {
        //     Console.Write("[emergency] ");
        //     device.EmergencyStop();
        // }
        //
        // static void StartOfShift(ICalibratable device)
        // {
        //     Console.Write("[shift start] ");
        //     device.Calibrate();
        // }

        static void Main()
        {
            Robot robot = new Robot { Name = "R-1" };
            Conveyor belt = new Conveyor { Name = "C-4" };
            Scale scale = new Scale { Name = "W-7" };

            // EmergencyButton(robot);
            // EmergencyButton(belt);
            // StartOfShift(robot);
            // StartOfShift(scale);
        }
    }
}
