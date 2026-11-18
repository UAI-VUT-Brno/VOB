using ProductionLine.Lines;
using ProductionLine.Models;

namespace ProductionLine
{
    class Program
    {
        static void Main()
        {
            AssemblyLine line = new AssemblyLine();
            line.Add(new Machine("lathe", true));
            line.Add(new Machine("mill", false));
            line.Add(new Machine("press", true));
            line.Add(new Machine("robot", true));
        }
    }
}
