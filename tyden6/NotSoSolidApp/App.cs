namespace Solid;

using Solid.Workforce;

internal class App
{
    public void Run()
    {
        var worker = new RobotWorker("robot", 8);

        worker.Work();
        Console.WriteLine($"Pay: {worker.CalculatePay()}");
        worker.SaveToFile("worker.txt");
        worker.SendEmail("boss@company.com");
        worker.Eat();
    }
}