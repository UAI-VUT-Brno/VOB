namespace Solid.Workforce;

public class RobotWorker : Worker
{
    public RobotWorker(string workerType, int hours) : base(workerType, hours)
    {
    }

    public override void Eat()
    {
        throw new InvalidOperationException("Robot does not eat");
    }

    public override void Sleep()
    {
        throw new InvalidOperationException("Robot does not sleep");
    }
}