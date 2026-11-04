namespace BuiltInGenerics
{
    class Program
    {
        static void Main()
        {
            // one shift of oil temperatures, once as a List and once as an array
            List<double> monday = new List<double>();
            monday.Add(41.5);
            monday.Add(44.0);
            monday.Add(47.5);
            double[] tuesday = { 40.0, 52.0, 49.0, 43.0 };
        }
    }
}
