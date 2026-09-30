// VOB - Exercise 3, block 1.2 - inheritance, the other way round
// Three classes written one after another, by copy and paste.

namespace Oop0302
{
    public class Pump
    {
        public string Name;
        public float Power;           // kW
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

        public string Describe()
        {
            return "Pump " + Name + " (" + Power + " kW)";
        }
    }

    public class Fan
    {
        public string Name;
        public float Power;           // kW
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

        public string Describe()
        {
            return "Fan " + Name + " (" + Power + " kW)";
        }
    }

    public class Compressor
    {
        public string Name;
        public float Power;           // kW
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

        public string Describe()
        {
            return "Compressor " + Name + " (" + Power + " kW)";
        }
    }

    class Program
    {
        static void Main()
        {
            Pump p = new Pump { Name = "P-101", Power = 7.5f };
            Fan f = new Fan { Name = "V-3", Power = 1.1f };
            Compressor c = new Compressor { Name = "K-2", Power = 22f };

            p.Start();
            f.Start();
            c.Start();

            Console.WriteLine(p.Describe());
            Console.WriteLine(f.Describe());
            Console.WriteLine(c.Describe());
        }
    }
}
