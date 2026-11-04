namespace ToolLibrary
{
    class Program
    {
        static void Main()
        {
            // drill diameters in mm
            Dictionary<string, double> drills = new Dictionary<string, double>();
            drills.Add("D1", 8.0);
            drills.Add("D2", 10.5);
            drills.Add("D3", 12.0);

            // machines at the stations of the line
            Dictionary<int, string> stations = new Dictionary<int, string>();
            stations.Add(1, "press");
            stations.Add(2, "robot");
            stations.Add(3, "conveyor");
        }
    }
}
