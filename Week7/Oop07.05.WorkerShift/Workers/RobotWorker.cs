namespace WorkerShift.Workers
{
    public class RobotWorker : Worker
    {
        public override void Eat()
        {
            throw new InvalidOperationException("Robot does not eat");
        }

        public override void Sleep()
        {
            throw new InvalidOperationException("Robot does not sleep");
        }
    }
}
