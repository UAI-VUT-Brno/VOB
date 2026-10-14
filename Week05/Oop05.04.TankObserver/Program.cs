using TankObserver.Core;

namespace TankObserver
{
    class Program
    {
        static void Main()
        {
            Tank tank = new Tank();

            tank.Fill(500);
            tank.Fill(450);
        }
    }
}
