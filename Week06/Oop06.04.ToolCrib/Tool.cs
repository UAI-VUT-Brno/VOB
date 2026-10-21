namespace ToolCrib
{
    public class Tool
    {
        public string Name { get; set; }

        public void Check()
        {
            Console.WriteLine(Name + ": checked");
        }
    }
}
