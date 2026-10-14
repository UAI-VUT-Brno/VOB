namespace TankObserver.Core
{
    public class Tank
    {
        public double Level { get; private set; }

        public void Fill(double litres)
        {
            Level = Level + litres;
            Console.WriteLine("filling " + litres + " l");
        }
    }
}
