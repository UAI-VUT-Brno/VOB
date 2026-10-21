namespace ToolCrib
{
    class Program
    {
        static void Main()
        {
            List<Tool> tools = new List<Tool>();
            tools.Add(new Drill { Name = "D1" });
            tools.Add(new Grinder { Name = "G1" });
            tools.Add(new Drill { Name = "D2" });
        }
    }
}
