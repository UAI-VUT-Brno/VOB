using ProductionLine.Models;

namespace ProductionLine.Lines
{
    // the machines of one line, in order
    public class AssemblyLine
    {
        private List<Machine> _machines = new List<Machine>();

        public void Add(Machine machine)
        {
            _machines.Add(machine);
        }
    }
}
