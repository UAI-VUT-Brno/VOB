// VOB - Exercise 3, block 5 - encapsulation
// Everything is public, so every caller is free to break the tank.

namespace Oop0309
{
    public class Tank
    {
        public string Name;
        public float Capacity;        // litres
        public float Level;           // litres

        public void Print()
        {
            Console.WriteLine(Name + ": " + Level + " / " + Capacity + " l");
        }
    }

    class Program
    {
        static void Main()
        {
            Tank t = new Tank { Name = "T-1", Capacity = 200, Level = 50 };
            t.Print();

            t.Level = -80;
            t.Print();

            t.Level = 100000;
            t.Print();

            t.Capacity = 0;
            t.Print();
        }
    }
}
