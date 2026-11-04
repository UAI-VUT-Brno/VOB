namespace MachineOperator
{
    public class Machine
    {
        public string Name { get; set; }

        public virtual void Start()
        {
            Console.WriteLine(Name + ": machine started");
        }
    }
}
