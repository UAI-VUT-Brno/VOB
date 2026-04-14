namespace PackagingCellCycle
{
    class DrillOperation : Operation
    {
        public double DepthMm { get; }

        public DrillOperation(string name, double depthMm) : base(name)
        {
            DepthMm = depthMm;
        }
    }
}