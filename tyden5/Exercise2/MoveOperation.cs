namespace PackagingCellCycle
{
    class MoveOperation : Operation
    {
        public double DistanceMm { get; }

        public MoveOperation(string name, double distanceMm) : base(name)
        {
            DistanceMm = distanceMm;
        }
    }
}