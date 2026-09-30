// VOB - Exercise 3, block 7.1 - value types and reference types
// The same code twice, once with a struct and once with a class.

namespace Oop0311
{
    public struct PointStruct
    {
        public float X;
        public float Y;
    }

    public class PointClass
    {
        public float X;
        public float Y;
    }

    class Program
    {
        static void MoveIt(PointStruct p)
        {
            p.X = p.X + 100;
        }

        static void MoveIt(PointClass p)
        {
            p.X = p.X + 100;
        }

        static void Main()
        {
            // --- struct -------------------------------------------------
            PointStruct s1 = new PointStruct { X = 1, Y = 1 };
            PointStruct s2 = s1;
            s2.X = 50;
            MoveIt(s1);

            Console.WriteLine("struct s1.X = " + s1.X);
            Console.WriteLine("struct s2.X = " + s2.X);

            // --- class --------------------------------------------------
            PointClass c1 = new PointClass { X = 1, Y = 1 };
            PointClass c2 = c1;
            c2.X = 50;
            MoveIt(c1);

            Console.WriteLine("class  c1.X = " + c1.X);
            Console.WriteLine("class  c2.X = " + c2.X);
        }
    }
}
