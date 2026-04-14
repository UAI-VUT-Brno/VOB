using System.Collections.Generic;

namespace PackagingCellCycle
{
    class Program
    {
        static void Main()
        {
            List<Operation> operations = new List<Operation>
            {
                new MoveOperation("Transfer to fixture", 250),
                new HeatOperation("Seal heating", 180),
                new DrillOperation("Vent hole", 8),
                new MarkOperation("Product code marking", "A17X")
            };

            CycleRunner runner = new CycleRunner();
            runner.Run(operations);
        }
    }
}