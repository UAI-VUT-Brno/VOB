using BrnoTransit.Data;
using BrnoTransit.Models;

namespace BrnoTransit
{
    class Program
    {
        static void Main()
        {
            // the real timetable: downloaded once, then read from the copy in the data folder
            string zip = GtfsSource.GetZip("data");
            List<Connection> connections = GtfsReader.ReadConnections(zip, GtfsReader.Tram);

            Console.WriteLine(connections.Count + " direct tram rides between neighbouring stops, for example:");
            foreach (Connection connection in connections.GetRange(0, Math.Min(8, connections.Count)))
            {
                Console.WriteLine("  " + connection.Line + ": " + connection.From + " -> " + connection.To
                                  + ", " + connection.Minutes + " min, " + connection.Trips + " trips");
            }
        }
    }
}
