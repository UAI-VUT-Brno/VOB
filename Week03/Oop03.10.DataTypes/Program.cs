// VOB - Exercise 3, block 6 - data types
// Predict every line before you run it.

namespace Oop0310
{
    class Program
    {
        static void Main()
        {
            // --- 1 -----------------------------------------------------
            int a = 2147483647;
            a = a + 1;
            Console.WriteLine("1: " + a);

            // --- 2 -----------------------------------------------------
            double sum = 0.1 + 0.2;
            Console.WriteLine("2: " + sum);
            Console.WriteLine("2: " + (sum == 0.3));

            // --- 3 -----------------------------------------------------
            int pieces = 7;
            int boxes = 2;
            Console.WriteLine("3: " + (pieces / boxes));

            // --- 4 -----------------------------------------------------
            double d = 3.99;
            int cut = (int)d;
            Console.WriteLine("4: " + cut);

            // --- 5 -----------------------------------------------------
            int big = 300;
            byte b = (byte)big;
            Console.WriteLine("5: " + b);

            // --- 6 -----------------------------------------------------
            var x = 10;
            var y = "10";
            Console.WriteLine("6: " + (x + x) + " " + (y + y));
        }
    }
}
