using System;
using System.Collections.Generic;

namespace PackagingCellCycle
{
    class CycleRunner
    {
        public void Run(List<Operation> operations)
        {
            foreach (Operation operation in operations)
            {
                if (operation is MoveOperation move)
                {
                    Console.WriteLine($"Move operation {move.Name}: moved part by {move.DistanceMm} mm.");
                }
                else if (operation is HeatOperation heat)
                {
                    Console.WriteLine($"Heat operation {heat.Name}: heated part to {heat.TemperatureC} C.");
                }
                else if (operation is DrillOperation drill)
                {
                    Console.WriteLine($"Drill operation {drill.Name}: drilled hole to depth {drill.DepthMm} mm.");
                }
                else if (operation is MarkOperation mark)
                {
                    Console.WriteLine($"Mark operation {mark.Name}: marked part with text '{mark.Text}'.");
                }
                else
                {
                    Console.WriteLine($"Unknown operation {operation.Name}");
                }
            }
        }
    }
}