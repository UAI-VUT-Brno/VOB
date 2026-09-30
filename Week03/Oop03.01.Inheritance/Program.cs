// VOB - Exercise 3, block 1.1 - inheritance
// A drill and a saw are both tools. What every tool has is already written.

namespace Oop0301
{
    public class Tool
    {
        public string Name;
        public float Weight;          // kg

        public string Describe()
        {
            return Name + ", " + Weight + " kg";
        }
    }

    // your Drill and Saw go here

    class Program
    {
        static void Main()
        {
            Tool hammer = new Tool { Name = "hammer", Weight = 0.8f };
            Console.WriteLine(hammer.Describe());

            // create a drill and a saw here
        }
    }
}
