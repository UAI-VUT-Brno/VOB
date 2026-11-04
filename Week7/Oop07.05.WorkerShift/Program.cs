using WorkerShift.Workers;

namespace WorkerShift
{
    class Program
    {
        static void Main()
        {
            List<Worker> shift = new List<Worker>();
            shift.Add(new Worker { Type = "human", Hours = 8 });
            shift.Add(new RobotWorker { Type = "robot", Hours = 8 });

            foreach (Worker worker in shift)
            {
                worker.Work();
                Console.WriteLine("pay: " + worker.CalculatePay());
                worker.SaveToFile(worker.Type + ".txt");
                worker.SendEmail("boss@company.com");
                worker.Eat();
            }
        }
    }
}
