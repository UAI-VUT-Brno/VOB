namespace TankObserver.Monitoring;

class Alarm : IVoyeur
{
    public void UpdateSelf(double level)
    {
        if (level>900)
        {
            Console.WriteLine("Warnign, high level!!!!!");
        }
    }
}