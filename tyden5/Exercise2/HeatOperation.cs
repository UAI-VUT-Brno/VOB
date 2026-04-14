namespace PackagingCellCycle
{
    class HeatOperation : Operation
    {
        public double TemperatureC { get; }

        public HeatOperation(string name, double temperatureC) : base(name)
        {
            TemperatureC = temperatureC;
        }
    }
}