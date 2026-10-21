namespace LabDevices
{
    public class Device
    {
        public string Name { get; set; }

        public void SwitchOn()
        {
            Console.WriteLine(Name + ": on");
        }
    }
}
