// VOB - Exercise 3, block 7.2 - a reference hiding inside an object

namespace Oop0312
{
    public class Order
    {
        public string Number;
        public List<string> Items = new List<string>();

        public Order Copy()
        {
            Order copy = new Order();
            copy.Number = Number;
            copy.Items = Items;
            return copy;
        }

        public void Print()
        {
            Console.WriteLine(Number + ": " + string.Join(", ", Items));
        }
    }

    class Program
    {
        static void Clear(List<string> items)
        {
            items.Clear();
        }

        static void Rename(string s)
        {
            s = "flange";
        }

        static void Main()
        {
            Order a = new Order { Number = "2026/001" };
            a.Items.Add("shaft");
            a.Items.Add("bearing");

            Order b = a.Copy();
            b.Number = "2026/002";
            b.Items.Add("gasket");

            a.Print();
            b.Print();

            Clear(a.Items);
            a.Print();

            string name = "shaft";
            Rename(name);
            Console.WriteLine("name = " + name);
        }
    }
}
