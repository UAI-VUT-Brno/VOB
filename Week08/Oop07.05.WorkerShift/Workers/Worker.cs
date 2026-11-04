namespace WorkerShift.Workers
{
    public class Worker
    {
        public string Type { get; set; }     // "human" or "robot"
        public int Hours { get; set; }

        public virtual void Work()
        {
            Console.WriteLine(Type + " working for " + Hours + " hours");
        }

        public virtual void Eat()
        {
            Console.WriteLine(Type + " eating");
        }

        public virtual void Sleep()
        {
            Console.WriteLine(Type + " sleeping");
        }

        public int CalculatePay()
        {
            if (Type == "human")
            {
                return Hours * 20;
            }
            if (Type == "robot")
            {
                return Hours * 15;
            }
            return 0;
        }

        public void SaveToFile(string path)
        {
            File.WriteAllText(path, "type=" + Type + ",hours=" + Hours);
            Console.WriteLine("saved to " + path);
        }

        public void SendEmail(string email)
        {
            Console.WriteLine("sending worker report to " + email);
        }
    }
}
