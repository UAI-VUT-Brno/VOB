using TankObserver.Core;
using TankObserver.Monitoring;

namespace TankObserver
{
    class Program
    {
        static void Main()
        {
            Tank tank = new Tank();
            Display display = new();
            var alarm = new Alarm();

            tank.AddVoyeur(display);
            tank.AddVoyeur(alarm);

            tank.Fill(500);
            Console.WriteLine("after first");
            tank.Fill(450);
            Console.WriteLine("after second");
        }
    }
}
