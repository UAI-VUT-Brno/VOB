namespace ProductionLine
{
    public class Machine
    {
        public string Name { get; set; }

        public void Start()
        {
            Console.WriteLine(Name + ": power on");
        }
    }
}
