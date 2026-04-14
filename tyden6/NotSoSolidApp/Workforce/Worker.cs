namespace Solid.Workforce;

public class Worker
{
    public string WorkerType { get; }
    public int Hours { get; }

    public Worker(string workerType, int hours)
    {
        WorkerType = workerType;
        Hours = hours;
    }

    public virtual void Work()
    {
        Console.WriteLine($"{WorkerType} working for {Hours} hours");
    }

    public virtual void Eat()
    {
        Console.WriteLine($"{WorkerType} eating");
    }

    public virtual void Sleep()
    {
        Console.WriteLine($"{WorkerType} sleeping");
    }

    public decimal CalculatePay()
    {
        if (WorkerType == "human")
            return Hours * 20;

        if (WorkerType == "robot")
            return Hours * 15;

        return 0;
    }

    public void SaveToFile(string path)
    {
        File.WriteAllText(path, $"type={WorkerType},hours={Hours}{Environment.NewLine}");
    }

    public void SendEmail(string email)
    {
        Console.WriteLine($"Sending worker report to {email}");
    }
}
