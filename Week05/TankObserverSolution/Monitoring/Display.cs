namespace TankObserver.Monitoring;
class Display : IVoyeur
{
    public void UpdateSelf(double level)
    {
        Console.WriteLine("UpdatingDisplay");
        Console.WriteLine(level);
    }
}