namespace Greenhouse.Plants
{
    public class Plant
    {
        public string Name;
        public int Water;                 // ml received so far

        public Plant(string name)
        {
            Name = name;
        }

        public void Print()
        {
            Console.WriteLine(Name + ": " + Water + " ml of water");
        }
    }
}
